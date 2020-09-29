# sqlserver死锁排查与解决

## 死锁的四个必要条件

- 互斥条件(Mutual exclusion)：资源不能被共享，只能由一个进程使用。
- 请求与保持条件(Hold and wait)：已经得到资源的进程可以再次申请新的资源。
- 非剥夺条件(No pre-emption)：已经分配的资源不能从相应的进程中被强制地剥夺。
- 循环等待条件(Circular wait)：系统中若干进程组成环路，该环路中每个进程都在等待相邻进程正占用的资源。

----

## SQL排查

### 查看指定数据库锁表进程

```sql
select spId
from master..SysProcesses
where db_Name(dbID) = '数据库名称'
and spId <> @@SpId
and dbID <> 0
```

### 查看被锁表

```sql
SELECT
    request_session_id spid,
    OBJECT_NAME( resource_associated_entity_id ) tableName
FROM
    sys.dm_tran_locks
WHERE
    resource_type = 'OBJECT'
```

<kbd>spid</kbd> 锁表进程

<kbd>tableName</kbd> 被锁表名

## 图形化监听

sqlserver -->工具--> sql server profiler，跟踪属性中选择<kbd>Deadlock graph</kbd>、<kbd>Lock:Deadlock</kbd>

## 日志追踪（errlog）

以全局方式打开指定的跟踪标记

```sql
DBCC TRACEON(1222,-1)

DBCC TRACEON(1204,-1)
```

使用 <kbd>EXEC master..xp_readerrorlog</kbd> 查看日志。 由于记录的死锁信息太多，贴出几个重点说下

```sql
Deadlock encountered .... Printing deadlock information
Wait-for graph
NULL
Node:1
PAGE: 7:1:6229275 CleanCnt:2 Mode:IX Flags: 0x3
Grant List 3:
Owner:0x00000004E99B7880 Mode: IX Flg:0x40 Ref:1 Life:02000000 SPID:219 ECID:0 XactLockInfo: 0x0000000575C7E970
SPID: 219 ECID: 0 Statement Type: UPDATE Line #: 84
Input Buf: Language Event: exec proc_PUB_StockDataImport
Requested by: 
ResType:LockOwner Stype:'OR'Xdes:0x0000000C7A905D30 Mode: U SPID:64 BatchID:0 ECID:59 TaskProxy:(0x0000000E440AAFE0) Value:0x8d160240 Cost:(0/0)
NULL

Node:2
PAGE: 7:1:5692366 CleanCnt:2 Mode:U Flags: 0x3
Grant List 3:
Owner:0x0000000D12099B80 Mode: U Flg:0x40 Ref:0 Life:00000001 SPID:64 ECID:0 XactLockInfo: 0x000000136B4758F0
SPID: 64 ECID: 0 Statement Type: UPDATE Line #: 108
Input Buf: RPC Event: Proc [Database Id = 7 Object Id = 907150277]
```

node:1 部分显示的几个关键信息：

- PAGE 7:1:6229275  （所在数据库ID 7， 1分区， 6229275行数）

- Mode: IX  锁的模式  意向排它锁

- SPID: 219  进程ID

- Event: exec proc_PUB_StockDataImport  执行的存储过程名

node:2 部分显示的几个关键信息

- PAGE 7:1:5692366  （所在数据库ID 7， 1分区，5692366行数）

- Mode:U 锁的模式  更新锁

- RPC Event: Proc 远程调用

- SPID: 64  进程ID

```sql
Victim Resource Owner:
ResType:LockOwner Stype:'OR'Xdes:0x0000000C7A905D30 Mode: U SPID:64 BatchID:0 ECID:59 TaskProxy:(0x0000000E440AAFE0) Value:0x8d160240 Cost:(0/0)
deadlock-list
deadlock victim=process956f4c8
process-list
process id=process956f4c8 taskpriority=0 logused=0 waitresource=PAGE: 7:1:6229275 waittime=2034 ownerId=2988267079 transactionname=UPDATE
lasttranstarted=2018-04-19T13:54:00.360 XDES=0xc7a905d30 lockMode=U schedulerid=24 kpid=1308 status=suspended spid=64 sbid=0 ecid=59 priority=0 trancount=0 
lastbatchstarted=2018-04-19T13:53:58.033 lastbatchcompleted=2018-04-19T13:53:58.033 clientapp=.Net SqlClient Data Provider hostname=VMSERVER76 hostpid=16328 
isolationlevel=read committed (2) xactid=2988267079 currentdb=7 lockTimeout=4294967295 clientoption1=671088672 clientoption2=128056
executionStack
frame procname=Test.dbo.proc_CnofStock line=108 stmtstart=9068 stmtend=9336 sqlhandle=0x03000700c503123601ba25019ca800000100000000000000
update dbo.pub_stock
set UpdateTime=GETDATE()
from pub_stock a
join PUB_PlatfromStocktemp b on a.GUID=b.StockGuid
```

从上面的信息能看到kill 掉的是进程id是process956f4c8

- 进程spid=64

- lockMode=U 获取更新锁

- isolationlevel=read committed

executionStack 执行的堆信息：

- 存储名 procname=Test.dbo.proc_CnofStock

- 语句 update dbo.pub_stock set UpdateTime=GETDATE() ...

- clientapp 发起事件的来源

----

## 解除锁

```sql
exec ('Kill '+cast(@spid as varchar))
```

```sql
declare @spid int
Set @spid = 57 --锁表进程
declare @sql varchar(1000)
set @sql='kill '+cast(@spid as varchar)
exec(@sql)
```

----

## 总结

### 避免死锁的解决方法

- 按同一顺序访问对象。

- 优化索引,避免全表扫描,减少锁的申请数目.

- 避免事务中的用户交互。

- 使用基于行版本控制的隔离级别。

- 将事务默认隔离级别的已提交读改成快照 `SET TRANSACTION ISOLATION LEVEL SNAPSHOT`

- 使用nolock去掉共享锁,但死锁发生在u锁或x锁上，则nolock不起作用

- 升级锁颗粒度(页锁，表锁), 以阻塞还代替死锁

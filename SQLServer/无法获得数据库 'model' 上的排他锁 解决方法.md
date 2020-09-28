# 无法获得数据库 'model' 上的排他锁 解决方法

创建数据库时提示错误：无法获得数据库 'model' 上的排他锁。请稍后重试该操作
由错误提示看出'model'模版数据库被其他进程占用了。

解决思路：
用查看系统进程语句查看model数据库被哪些进程占用了，找到进程id,然后用kill命令杀掉占用进程
 
步骤：
- 使用以下语句查出占用model数据库的进程id，然后使用kill命令 杀掉进程
- 查看占用model数据库的进程，如果是2000，替换成master.dbo.sysprocesses

```sql
use master  --选择数据库
go
select spid from master.sys.sysprocesses where dbid = db_id('model');

--杀掉占用model数据库的进程
use master  --选择数据库
go
declare @sql varchar(100) 
while 1=1 
begin 
  select top 1 @sql = 'kill '+cast(spid as varchar(3)) 
  from master..sysprocesses where spid > 50 and spid <> @@spid and  dbid = db_id('model')
  if @@rowcount = 0 
    break ;
  print(@sql)   --打印杀掉进程语句
  exec(@sql)    --执行杀掉进程语句
end
go
 ```
以上语句成功执行后，再执行先前的数据库创建脚本，数据库成功创建，问题解决.

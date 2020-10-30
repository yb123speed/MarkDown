---
marp: true
---
# 数据库性能优化

----

## SQL Server Profiler 查看EFCore生成的SQL

----

1. 在项目appsettings.json中的数据库连接字符串中追加`App=EFCore;`，其中**EFCore**可以随便写。
2. 启动SQL Server Profiler,主界面点击左上角的**文件**，连接到服务器。
3. 在弹出的界面中切换到选项卡**事件选择**，除**RPC:Completed**之外全部都不选，记得勾上**TextData**列，点击右下角的**列筛选器**。
4. 在弹出的**编辑筛选器**界面的第一个**ApplicationName**的*类似于*中加上`EFCore`，点击**确认**,关闭弹窗。
5. 点击**运行**，可以看到SQLServer执行的EFCore生产的SQL。

----

![新建跟踪](./images/sql/0x00-sqlserverprofiler.png)

----

[^_^]:
    [EFCore 5.0将支持LogTo方法输出SQL](https://docs.microsoft.com/en-us/ef/core/miscellaneous/events/simple-logging)

```csharp
protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    => optionsBuilder.LogTo(Console.WriteLine);
```

----

## 写好SQL

----

xxx

----

## 索引

----

SQL Server索引类型：唯一索引，主键索引，聚集索引，非聚集索引

----

### 聚合索引和非聚合索引

| 动作描述 | 使用聚集索引 | 使用非聚集索引 |
| -- | -- | -- |
列经常被分组排序 | 应 | 应
返回某范围内的数据 | 应 |不应
一个或极少不同值 | 不应| 不应
小数目的不同值| 应 |不应
大数目的不同值 |不应| 应
频繁更新的列 |不应 |应
外键列 | 应 | 应
主键列 | 应 |应
频繁修改索引列 |不应 | 应

----

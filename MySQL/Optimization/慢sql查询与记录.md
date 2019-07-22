# 慢查询sql的检测与记录

## 前提

在系统的优化过程中，对SQL语句的优化更是非常重要的一部分。系统上线后发现了许多对性能有影响的SQL。本文主要是讲解利用MySQL的一些状态分析并且记录影响数据库性能的语句。

## MySQL状态查询

```sql
show status like 'com_insert';-- 显示执行了多少次插入操作
show status like 'com_update';-- 显示执行了多少次更新
show status like 'com_delete';-- 显示执行了多少次删除
show status like 'com_select';-- 显示执行多少次查询
show status like 'uptime';-- 显示mysql数据库启动多长时间，如果时间很长，数据库表的存储引擎是MyISAM，这个时候要注意碎片整理。
```

## 显示慢查询

### 显示慢查询需要先开始慢查询，MySQL中慢查询默认为关闭状态，查询慢SQL是否开启

```sql
show variables like 'slow_query_log';-- 默认为OFF状态
```

### 查询记录没有使用索引的查询是否开启（默认没有开启）

```sql
show variables like 'log_queries_not_using_indexes';
```

### 查询时间慢查询的sql语句的时长（默认10秒）

```sql
show variables like 'long_query_time'; -- 显示慢查询的时间，默认情况下是10秒一个慢查询
```

### 开启慢查询后查询慢查询的条数

```sql
show status like 'slow_queries';-- 显示慢查询的条数
```

### 查看记录慢查询的sql的位置

```sql
show variables like 'slow_query_log_file'
```

### 查看正在执行的sql语句

```sql
select * from information_schema.`PROCESSLIST` where info is not null;
```

----

## 修改配置

### 1.修改MySQL配置文件（需要重启mysql服务，但是配置可以持续保留）

```ini
#开启慢查询日志记录  
slow_query_log=1  
#查询时间超过0.1秒的sql语句会被记录  
long_query_time=0.1  
#记录没有使用索引的查询  
log_queries_not_using_indexes=1  
#记录慢查询日志的文件地址  
slow-query-log-file=/var/lib/mysql/localhost-slow.log
```

### 2.（在mysql控制台修改,无需重启mysql服务，但是配置在MySQL服务重启后就失效了）

```shell
#开启慢查询日志记录
mysql> set global slow_query_log=on;
#查询时间超过0.1秒的sql语句会被记录
mysql> set long_query_time=0.1;
#记录慢查询日志的文件地址
mysql> set global slow_query_log_file="/var/lib/mysql/localhost-slow.log";
#记录没有使用索引的查询
mysql> set global log_queries_not_using_indexes=on;
```
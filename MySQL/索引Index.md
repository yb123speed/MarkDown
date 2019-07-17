# 索引

数据库的索引就像一本书的目录，能够加快数据库的查询速度。

## 索引类型

MySQL索引有四种PRIMARY、NORMAL(INDEX)、UNIQUE、FULLTEXT， 其中PRIMARY、NORMAL(INDEX)、UNIQUE是一类，FULLTEXT是一类。
这四种都是单列索引，也就是他们都是作用于单个一列，所以也称单列索引；但是所以一个索引也可以作用于多个列上，称为组合索引或复合索引。

- primary：唯一索引，不允许为null。**一张表只能有一个主键索引（主键索引通常在建表的时候就指定）**

- index：普通非唯一索引。**索引列没有任何限制**

- unique：表示唯一的，**不允许重复的索引，可以为null**。

- fulltext: 表示全文搜索的索引。 FULLTEXT用于搜索很长一篇文章的时候，效果最好。用在比较短的文本，如果就一两行字的，普通的INDEX 也可以。

- spatial：空间索引。(一般用不到)

## 单列索引

新建一张测试表

```sql
CREATE TABLE T_USER( ID INT NOT NULL,USERNAME VARCHAR(16) NOT NULL);
```

(1)PRIMARY：主键索引

```sql
CREATE TABLE T_USER(ID INT NOT NULL,USERNAME VARCHAR(16) NOT NULL,PRIMARY KEY(ID))
```

(2)NORMAL：普通索引。索引列没有任何限制；

建表时指定

```sql
CREATE TABLE T_USER(ID INT NOT NULL,USERNAME VARCHAR(16) NOT NULL,INDEX USERNAME_INDEX(USERNAME(16))) //给列USERNAME建普通索引USERNAME_INDEX
```

ALTER语句指定

```sql
ALTER TABLE T_USER ADD INDEX U_INDEX (USERNAME) //给列USERNAME建普通索引 U_INDEX
```

删除索引

```sql
DROP INDEX U_INDEX ON t_user  //删除表t_user中的索引U_INDEX
```

(3)UNIQUE：唯一索引。索引列的值必须是唯一的，但允许有空；

建表时指定

```sql
CREATE TABLE t_user(ID INT NOT NULL,USERNAME VARCHAR(16) NOT NULL,UNIQUE U_INDEX(USERNAME)) //给列USERNAME添加唯一索引T_USER
```

ALTER语句指定

```sql
ALTER TABLE t_user ADD UNIQUE u_index(USERNAME) //给列T_USER添加唯一索引u_index
```

删除索引

```sql
DROP INDEX U_INDEX ON t_user
```

(4)FULLTEXT：全文搜索的索引。**FULLTEXT 用于搜索很长一篇文章的时候，效果最好。用在比较短的文本，如果就一两行字的，普通的 INDEX 也可以。**索引的新建和删除和上面一致，这里不再列举...

## 组合索引（复合索引）　　

新建一张表

```sql
CREATE TABLE T_USER(ID INT NOT NULL,USERNAME VARCHAR(16) NOT NULL,CITY VARCHAR(10),PHONE VARCHAR(10),PRIMARY KEY(ID) )
```

组合索引就是把多个列加入到统一个索引中，如新建的表T_USER，我们给USERNAME+CITY+PHONE创建一个组合索引

```sql
ALTER TABLE t_user ADD INDEX name_city_phone(USERNAME,CITY,PHONE)  //组合普通索引
```

```sql
ALTER TABLE t_user ADD UNIQUE name_city_phone(USERNAME,CITY,PHONE) //组合唯一索引
```

这样的组合索引，其实相当于分别建立了三个索引。
- USERANME,CITY,PHONE
- USERNAME,CITY
- USERNAME,PHONE
 
为什么没有（CITY,PHONE）索引呢？这是因为MYSQL组合查询“**最左前缀**”的结果。**简单的理解就是只从最左边开始组合**。
 
并不是查询语句包含这三列就会用到该组合索引：

这样的查询语句才会用到创建的组合索引

```sql
SELECT * FROM t_user where USERNAME="parry" and CITY="广州" and PHONE="180"
SELECT * FROM t_user where USERNAME="parry" and CITY="广州"
SELECT * FROM t_user where USERNAME="parry" and PHONE="180" 
```

这样的查询语句是不会用到创建的组合索引　

```sql
SELECT * FROM t_user where CITY="广州" and PHONE="180"
SELECT * FROM t_user where CITY="广州"
SELECT * FROM t_user where PHONE="180"
```

## 索引不足之处

- 索引提高了查询的速度，但是降低了INSERT、UPDATE、DELETE的速度，因为在插入、修改、删除数据时，还要同时操作一下索引文件；
- 建立索引会占用一定的磁盘空间。
- 当对表中的数据进行增加、删除和修改的时候，索引也要动态的维护，这样就降低了数据的维护速度。

##  索引使用注意事项

1. 只要列中包含NULL值将不会被包含在索引中，组合索引只要有一列含有NULL值，那么这一列对于组合索引就是无效的，所以我们在设计数据库的时候最好不要让字段的默认值为NULL;
2. 使用短索引。如果可能应该给索引指定一个长度，例如：一个VARCHAR(255)的列，但真实储存的数据只有20位的话，在创建索引时应指定索引的长度为20，而不是默认不写。如下

    ```sql
    ALTER TABLE t_user add INDEX U_INDEX(USERNAME(16))
     优于 
    ALTER TABLE t_user add INDEX U_INDEX(USERNAME)
    ```

    使用短索引不仅能够提高查询速度，而且能节省磁盘操作以及I/O操作。

3. 索引列排序。
    
    Mysql在查询的时候只会使用一个索引，因此如果where子句已经使用了索引的话，那么order by中的列是不会使用索引的，所以order by尽量不要包含多个列的排序，如果非要多列排序，最好使用组合索引。
4. Like 语句。
        
    一般情况下不是鼓励使用like,如果非使用，那么需要注意 like"%aaa%"不会使用索引；但like“aaa%”会使用索引。

5. 不使用 NOT IN和<>操作

## 索引方式 HASH和 BTREE比较

1. HASH

    用于对等比较，如"="和" <=>"

2. BTREE

    BTREE索引看名字就知道索引以树形结构存储，通常用在像 "=，>，>=，<，<=、BETWEEN、Like"等操作符查询效率较高；

    通过比较发现，我们常用的是BTREE索引方式，当然Mysql默认就是BTREE方式。

## 创建索引的准则

索引是建立在数据库表中的某些列的上面。因此，在创建索引的时候，应该仔细考虑在哪些列上可以创建索引，在哪些列上不能创建索引。

一般来说，应该在这些列上创建索引。
1. 在经常需要搜索的列上，可以加快搜索的速度；
2. 在作为主键的列上，强制该列的唯一性和组织表中数据的排列结构；
3. 在经常用在连接的列上，这些列主要是一些外键，可以加快连接的速度；
4. 在经常需要根据范围进行搜索的列上创建索引，因为索引已经排序，其指定的范围是连续的；
5. 在经常需要排序的列上创建索引，因为索引已经排序，这样查询可以利用索引的排序，加快排序查询时间；
6. 在经常使用在WHERE子句中的列上面创建索引，加快条件的判断速度。

## 参考

[Mysql索引PRIMARY、NORMAL、UNIQUE、FULLTEXT 区别和使用场合](https://www.cnblogs.com/parryyang/p/5900926.html)

[mysql索引类型 normal, unique, full text](https://www.cnblogs.com/wmm123/p/8549512.html)
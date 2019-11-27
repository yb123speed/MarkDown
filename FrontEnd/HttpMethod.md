# HTTP Method

## GET

   通常用于请求服务器发送某个资源。HTTP/1.1实现此方法。

## HEAD

   HEAD与GET行为类似，`但服务器在响应中只返回首部。不会返回报文的主体部分。`这就允许客户端在未获得实际资源的情况下，`对资源的首部进行检查。`

## PUT

   `与GET方法从服务器读取文档相反，PUT方法会向服务器写入文档。`有些发布系统允许用户创建WEB页面，并用PUT直接将其安装到WEB服务器上。

## POST

   `POST方法起初是用来向服务器写入数据的。`实际上，通常会用它来支持HTML的表单。表单中填好的数据通常会被发送给服务器，然后服务器将其发送到他要去的地方。

## TRACE

   TRACE方法允许客户端在最终将请求发送给服务器时，看看他变成了什么样子。

   TRACE请求最终会在目的服务器发起一个回环诊断，行程最后一站的服务器会弹回一条TRACE响应，并在响应主体中携带它收到的原始请求报文。这样客户端就可以查看在所有中间HTTP程序组成的请求响应链上，原始报文是否以及如何被毁坏或修改过。

   TRACE方法主要用于诊断

   中间应用程序会自行决定对TRACE请求的处理方式

   TRACE请求不能带有实体的主体部分。TRACE响应的实体主体部分包含了响应服务器收到的请求的精确副本。

## OPTIONS

   OPTIONS方法请求WEB服务器告知其支持的各种功能。

   可以询问服务器通常支持哪些方法，或者对某些特殊资源支持哪些方法。

   使用OPTIONS方法的请求和响应示例：

   请求报文

   ```html
   OPTIONS http://www.cnivi.com.cn/ HTTP/1.1
   Accept-Encoding: gzip,deflate
   Host: www.cnivi.com.cn
   Connection: Keep-Alive
   User-Agent: Apache-HttpClient/4.1.1 (java 1.5)
   ```

   响应报文

   ```
   HTTP/1.1 200 OK
   Server: Apache-Coyote/1.1
   Allow: GET, HEAD, POST, PUT, DELETE, TRACE, OPTIONS, PATCH
   Content-Length: 0
   Date: Thu, 09 Oct 2014 04:20:09 GMT
   ```

## DELETE

   `DELETE方法所做的事情就是请服务器删除请求URL所指定的资源。`

   但是客户端应用程序无法保证删除操作一定会执行。因为HTTP规范允许服务器在不通知客户端的情况下撤销请求。

## LOCK

   允许用户锁定资源，比如可以在编辑某个资源时将其锁定，以防别人同时对其进行编辑。

## MKCOL

   允许用户创建资源

## COPY

    便于用户在服务器上复制资源

## MOVE

    在服务器上移动资源



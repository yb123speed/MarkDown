# 跨站请求伪造与 SameSite Cookie

## 跨站请求伪造

跨站请求伪造（又被称为 CSRF 或者 XSRF ），它源自一个域网站向另一个域网站发起请求的简单功能。攻击者通过一些技术手段欺骗用户使用浏览器去访问一个自己曾经认证过的网站并执行一些敏感操作（如转账）。

一个域网站向另一个域的网站发起请求的方式有很多，例如点击一个超链接、加载静态资源、提交表单以及直接发起 ajax 请求等。如：

```html
<a href="http://a.com/xx">点击有惊喜</a>    # 诱导用户点击

<img src="http://a.com/xx">     # 浏览器默认加载资源 - 图片

    <link href="http://a.com/xx" rel="stylesheet">    # 浏览器默认加载资源 - css 文件

        <form method="post" action="http://a.com/xx">    # 构造可以提交的表单
            <input type="text" name="name" value="value">
                <input type="submit">
                </form>
```

如果用户之前在 a.com 认证过，即浏览器保持有效的 cookie ，这些请求也会携带相应的 cookie ，而用户可能并不知情。

## SameSite Cookie

Same-Site Cookies 出现以前我们并没有一种简单而有效的方式去阻止 CSRF 攻击，其中一种方式是通过检查 `origin` 和 `referer` 来校验，缺点是依赖浏览器发送正确的字段，而这并不总是准确有效的；另一种方式则是通过给表单添加随机 `token` 的方式来校验，但是部署比较麻烦。

SameSite Cookie 的出现就是为了解决这个问题，它可以完全有效的阻止 CSRF 攻击。SameSite Cookie 非常容易部署，只需要将你原来的设置 cookie 的地方，如下：

```http
Set-Cookie: key=value; path=/
```

改为：

```http
Set-Cookie: key=value; path=/; SameSite
```

准确的说 `SameSite` 这个属性有三个可选值，分别是 `Strict`、 `Lax`、`None` 。其中 `Strict` 为严格模式，另一个域发起的任何请求都不会携带该类型的 cookie，能够完美的阻止 CSRF 攻击，但是也可能带来了少许不便之处，例如通过一个导航网站的超链接打开另一个域的网页会因为没有携带 cookie 而导致没有登录等问题。因此 `Lax` 相对于 `Strict` 模式来说，放宽了一些。简单来说就是，用安全的 HTTP 方法（GET、HEAD、OPTIONS 和 TRACE）改变了当前页面或者打开了新页面时，可以携带该类型的 cookie。`None` 必须配合`Secure`使用，必须使用https传输。具体见下表：

| 请求类型 | 例子 | 非SameSite | SameSite = Lax | SameSite = Strict |
| -- | -- | -- | -- | -- |
link|`<a href="…">`|Y|Y|N
prerender|`<link rel="prerender" href="…">`|Y|Y|N
form get|`<form method="get" action="…">`|Y|Y|N
form post|`<form method="post" action="…">`|Y|N|N
iframe|`<iframe src="…">`|Y|N|N
ajax|`$.get('…')`|Y|N|N
image|`<img src="…">`|Y|N|N

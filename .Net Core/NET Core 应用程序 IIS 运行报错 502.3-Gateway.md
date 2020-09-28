# .NET Core 应用程序 IIS 运行报错 502.3-Gateway

将 NET Core 应用程序部署在 IIS 环境，默认配置下，如果任务执行时间长达 2 分钟，会收到如下错误（Bad Gateway）。

如果要执行长时间任务，可以修改发布后的 web.config 文件的 system.webServer / aspNetCore 节，为其添加 requestTimeout 属性：

```xml
<system.webServer>
  <aspNetCore requestTimeout="00:20:00" ... />
</system.webServer>
```

## 属性说明

- <kbd>requestTimeout</kbd>
  - 可选的 timespan 属性。
  - 指定 ASP.NET 核心模块将等待侦听 %aspnetcore_port%的进程的响应的持续时间。
  - 默认值为“00:02:00”。
  - `requestTimeout`  必须指定整分钟数，否则它将默认为 2 分钟。

## 参考

[ASP.NET Core Module configuration reference](https://docs.microsoft.com/zh-cn/aspnet/core/hosting/aspnet-core-module)

[.NET Core publish to IIS - HTTP Error 502.3 - Bad Gateway - The specified CGI application encountered an error and the server terminated the process](https://stackoverflow.com/questions/39756042/net-core-publish-to-iis-http-error-502-3-bad-gateway-the-specified-cgi-ap/48164725#48164725)

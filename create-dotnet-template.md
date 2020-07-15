# 创建自己的dotnet模板

## 1.准备自己的Demo项目模板

### 开发一个空的并编译通过的项目

1. 必须Code已经成型了，意思就是说，分层清楚，引用明确，只不过没有复杂的内容。
2. 必须编译成功，不能有任何错误，否则后边会各种麻烦。
3. 尽量能跑起来，就是能显示出界面，当然也不是必要条件，比如如果你新建了控制台程序，是没有web页面的。
4. 一般不要把生成的dll文件包含其中，可执行文件都过滤了。

## 2.将 Demo 模板导入到dotnet模块库中

在你的电脑任何地方，新建一个模板文件夹temple，用于以后打包多个模板使用，比如我是这样的（尽量按照这个格式来：content文件夹包含code模板）：

```shell
├── temple                               //  用来存放所有的模板
│   ├── YBlogCoreTemple                  //  YBlog.Core模板全部内容
│   │    ├── content                     //  存放Code 项目代码，可直接运行
│   │    │    ├── YBlog.Core
│   │    │    ├──  .
│   │    │    ├──  .
│   │    │    ├──  .
│   │    │    ├── YBlog.Core.Services
│   │    │    └── YBlog.Core.sln
│   │    │
│   │    ├── license                     //  存放版本许可信息，如果不添加，后边会警告，文章后边会提到
│   │    │    └── license.txt
│   │    │
│   │    └── 其他待定                     //  这里文章后边会打包的时候用到
│   │
│   └── DDDTemple                        //  DDD模板信息
```

用于定义模板的配置文件 (template.json)

1. 向源代码项目的根目录添加 .template.config 文件夹（注意是文件夹），到时候与它同级的文件都会被打包。
2. 在 .template.config 文件夹中，创建 template.json 文件来配置模板。

```json
{
    "$schema": "http://json.schemastore.org/template",//template.json 文件的 JSON 架构，可以不要该键值对
    "author": "yb123speed", //必填！模板创建者
    "classifications": [ "Web/WebAPI" ], //必填，这个对应模板的Tags，其他的比如 [ "Common", "Console" ],
    "name": "YBlog.Core Dotnet", //必填，这个是模板名，比如ASP.NET Core Web API
    "identity": "YBlog.Core.Template", //可选，模板的唯一名称
    "shortName": "yblogtpl", //必填，这个对应模板的短名称，比如webapi
    "tags": {
      "language": "C#" ,
      "type":"project"
    },
    "sourceName": "YBlog.Core",  // 可选，要替换的名字，这个就是模板的项目名，以后新建的时候，会自动把这个名字替换成你想要的任何，比如HelloBlog
    "preferNameDirectory": true  // 可选，添加目录  
}
```

提示：这个模板被执行分发，添加到 dotnet 模板后，尽量保存好，不要删掉，因为如果删掉后，如果以后想卸载这个本地的模板，就不能了。

若要从本地文件系统卸载模板，需要完全限定路径。 例如，C:\Users\<USER>\Documents\Templates\XXX.ConsoleTemplate.CSharp 有效。

详细信息可以查看[官网](https://docs.microsoft.com/zh-cn/dotnet/core/tools/custom-templates)

```shell
├── temple                               //  用来存放所有的模板
│   ├── YBlogCoreTemple                  //  YBlogCore模板全部内容
│   │    ├── content                     //  存放Code 项目代码，可直接运行
│   │    │    ├── .template.config       //  模板配置文件夹
│   │    │    │    └── template.json     //  配置文件
│   │    │    ├── YBlog.Core
│   │    │    ├──  .
│   │    │    ├──  .
│   │    │    ├──  .
│   │    │    ├── YBlog.Core.Services
│   │    │    └── YBlog.Core.sln
│   │    │
│   │    ├── license                     //  存放版本许可信息，如果不添加，后边会警告，文章后边会提到
│   │    │    └── license.txt
│   │    │
│   │    └── 其他待定                     //  这里文章后边会打包的时候用到
│   │
│   └── DDDTemple                        //  DDD模板信息
```

## 3.使用文件系统分发

```bat
// 使用文件分发模板，
// 注意文件路径：content文件夹的上一级，可以对比上边的截图中的文件夹结构
dotnet new -i D:\temple\YBlogCoreTemple
```

------

## 发布 Demo 项目到 Nuget

### 1.添加 nuspec 范本文件

在 content 文件夹同级，添加 nuspec 文件。 nuspec 文件是 XML 清单文件，用于描述包内容，并促进创建 NuGet 包。

```xml
<?xml version="1.0" encoding="utf-8"?>
<package xmlns="http://schemas.microsoft.com/packaging/2012/06/nuspec.xsd">
  <metadata>
    <id>YBlog.Core.Webapi.Template</id>// nuget包标识，在 nuget.org 或包驻留的任意库中必须是唯一的
    <version>1.0.0</version>// 遵循 major.minor.patch 模式的包版本。
    <description>
      Creates a blog core webapi app.// 用于 UI 显示的包的详细说明。
    </description>
    <authors>yb123speed</authors>// 包创建者的逗号分隔列表，与 nuget.org 上的配置文件名称一致
    <packageTypes>
      <packageType name="Template" />// 包类型
    </packageTypes>
    <license type="file">license\license.txt</license>// 上文提到的许可证信息
  </metadata>
</package>
```

### 2.下载Nuget.exe

上边配置好了，那怎么如何打包呢，现在咱们就需要 nuget.exe 工具了，[下载](https://www.nuget.org/downloads)最新的exe文件即可，注意这个不是安装文件，这个需要配合着项目使用，如果你双击是无效的，把下载好的 nuget.exe 拷贝到 nuspec  范本文件同级的目录中：

### 3.生成Nupkg包

文件也配置好了，nuget执行文件也下载好了，接下来咱们就是正式开始打包了：

打开 DOS 命令窗口，进入到当前文件夹，然后直接运行打包命令：（注意打包的文件，是咱们创建的 nuspec 范本文件）

```cmd
// 执行打包操作，文件地址就是 nuspec 范本地址
nuget pack D:\temple\YBlogCoreTemple\YBlog.Core.Webapi.Template.nuspec
```

### 4.发布到nuget.org（注意质量）

接下来就是最后一步了，将我们打包成功的 nupkg 包，发布到 nuget.org ，这里有多种方法。

首先你需要在 nuget.org 官网注册账号，这里不细说，然后点击到[上传页面](https://www.nuget.org/packages/manage/upload)

1. 选择 nuget.org 顶部菜单中的“上传”，并浏览到包位置。

![在 nuget.org 上上传包](images/create-dotnet-template/publish_uploadyourpackage.png)

nuget.org 告知包名称是否可用。 如果无法使用，则更改项目中的包标识符、重新生成，并重试上传。

如果包名称可用，nuget.org 将打开“验证”部分，可以在其中查看包清单中的元数据。 若要更改任何元数据，请编辑项目（项目文件或 .nuspec 文件）、重新生成、重新创建包，然后再次上传。

在“导入文档”下，可以粘贴 Markdown、将 URL 指向文档，或上传文档文件。

当所有信息准备就绪后，选择“提交”按钮

上传成功后，nuget 会后台进行扫描病毒，然后进行发布，中间大概等待10分钟后，你会收到一个官方的邮件，提示你已经发布成功。

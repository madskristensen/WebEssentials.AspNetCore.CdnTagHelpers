# ASP.NET Core CDN helpers

[![Build status](https://ci.appveyor.com/api/projects/status/txquc7aq1kgweap7?svg=true)](https://ci.appveyor.com/project/madskristensen/webessentials-aspnetcore-cdntaghelpers)
[![NuGet](https://img.shields.io/nuget/v/WebEssentials.AspNetCore.CdnTagHelpers.svg)](https://nuget.org/packages/WebEssentials.AspNetCore.CdnTagHelpers/)

This NuGet package makes it painless to use CDNs to serve static files on any ASP.NET Core web application.

## Get started
It's easy to use a CDN in your ASP.NET Core web application. Here's how to get started.

### 1. Setup a CDN
We recommend you use the [Azure CDN (Verizon)](https://azure.microsoft.com/en-us/services/cdn/), but any CDN supporting *reverse proxying* or *origin push* will work.

When using the Azure CDN, you will get an endpoint URL that looks something like this: `https://myname.azureedge.net`. You need that URL in step 2.

### 2. Register the Tag Helpers
Do that by adding this line to the **_ViewImports.cshtml** file:

```csharp
@addTagHelper *, WebEssentials.AspNetCore.CdnTagHelpers
```

Then add he CDN url to the appsettings.json file:

```json
{
  "cdn": {
    "url": "https://myname.azureedge.net"
  }
}
```

That's it. Now your should serve all your static assets from the CDN.

### 3. Verify it works
Run the page in the browser and make sure that all JavaScript, CSS and image references have been modified to now point to the CDN.

You can do that by looking at the HTML source code. There should no longer by any relative file references, like this one:

```html
<script src="js/site.js"></script>
```

...but instead it should look like this:

```html
<script src="https://myname.azureedge.net/js/site.js"></script>
```

## Configuration
# SysAdminsMedia.BlazorIconify
BlazorIconify is a Blazor component library for Iconify, a unified icon framework that provides a consistent icon experience across all platforms. BlazorIconify is a wrapper around the Iconify API that allows you to easily add icons to your Blazor applications.

## Installation
You can install BlazorIconify via NuGet. Run the following command in the Package Manager Console:
```
Install-Package SysAdminsMedia.BlazorIconify
```
or via the .NET Core CLI:
```
dotnet add package SysAdminsMedia.BlazorIconify
```

## Usage
First, you need to add the following line to your `_Imports.razor` file:
```csharp
@using SysAdminsMedia.BlazorIconify
```

Then, you can use the `Iconify` component in your Blazor components like this:
```html
<Iconify Icon="mdi:home" />
```

You can also adjust the color and other properties of the icon:
```html
<Iconify Icon="mdi:home" Color="red" Class="my-custom-class" Style="align-content: center;" />
```
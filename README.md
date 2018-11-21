## Welcome to LiveSharp

LiveSharp is a .NET tool that lets you develop your code without recompilation. 

1. Run your application
2. Change the code
3. Instantly see the result!

### How it works

During the build LiveSharp injects code in the beginning of the method. When this method is executed it first checks if there is an updated version exists. You can choose which methods LiveSharp injects by modifying the `livesharp.rules` file that is created after the initial build. The exact format is described further in this document.

### Installation

1. [Install Visual Studio extension](https://marketplace.visualstudio.com/items?itemName=ionoy.LiveSharp)
2. In Package Manager Console choose the project that you want runtime update to work on and install [LiveSharp NuGet package](https://www.nuget.org/packages/livesharp)
3. Build. You should see L# icon near the methods that allow runtime editing.
4. Run the application, change code, save file.

### 'livesharp.rules' file

This file contains all the information LiveSharp uses to work with your application. 
By default it contains only one line:
```
*
```
This means that all of the methods in current project are to be injected (enabled for LiveSharp). In some cases you might want to include only certain types. For example, when doing Xamarin Forms UI development these could be types deriving from ContentPage:
```
Xamarin.Forms.ContentPage
```
You can also include singular methods:
```
MyNamespace.MyType MethodName
```
If method has any overloads, you can choose one by providing parameter types:
```
MyNamespace.MyType MethodName System.Collections.Generic.Dictionary
```
You can use wildcards:
```
Xamarin.Forms.Content* Build*
```
This will match all methods starting with `Build` that are defined in types derived from types in `Xamarin.Forms` namespace starting with `Content`. Like `ContentPage` or `ContentView`.

#### Start rule

LiveSharp needs a starting point to initialize itself. For example, with Xamarin Forms it should be your App type constructor. But since you can have a different application structure, LiveSharp might not be able to resolve it automatically. 

To manually assign the initialization method you can use the following syntax:
```
@MyNamespace.App StartTheApplication
```

### Troubleshooting

`Warning: LiveSharp couldn't find start method.`

Open `livesharp.rules` file in the project root folder and add a line like this: `@MyApp.App .ctor`
Where `MyApp.App` is a full name of your Application type and `.ctor` is either a method name or a constructor identifier.

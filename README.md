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

### Troubleshooting

`Warning: LiveSharp couldn't find start method.`

LiveSharp needs a starting point to initialize itself. For example, with Xamarin Forms it should be your App type constructor. But since you can have a different application structure, LiveSharp might not be able to resolve it automatically. 

To solve it, open `livesharp.rules` file in the project root folder and add a line like this: `@MyApp.App .ctor`
Where `MyApp.App` is a full name of your Application type and `.ctor` is either a method name or a constructor identifier.

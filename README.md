# BlazorWorkshop


You will see several projects in the repository.

**Workshop.Client** contains all UI components for the application.  
**Workshop.Server** hosts the Blazor app and the backend services.  
**Workshop.Shared** contains shared model types  
**Workshop.ComponentsLibrary** contains helper code that will be used later


### Stage 1

Ensure that the server starts and works properly by building and running it. It should show a white page with the title "Drug Express" 

Open Pages/Index.razor in the `Client` project to see the code for the home page. Feel free to change the title and restart the server.  
  
Next, add a `@code` block to the file and initialize a list field to keep track of your specials:

  ```javascript
    @code {
        List<DrugSpecial> specials;
    }
  ```
The `DrugSpecial` type is already defined for you in the `Shared`-project

To fetch the available list of specials we need to call an API on the backend. Blazor provides ad preconfigured `HttpClient` through dependency injection that is already set up with the correct base address. You can use the `@inject` directive to inject an `HttpClient` into the component using dependency injection:

```javascript
    @page "/"
    @inject HttpClient HttpClient
```
Override the lifecycle hook `OnInitializedAsync` and use the `GetJsonAsync<T>()` method to get the specials and automatically deserialize the JSON into the `DrugSpecial` type.

```javascript
    @code {
        List<DrugSpecial> specials;

        protected async override Task OnInitializedAsync()
        {
            specials = await HttpClient.GetJsonAsync<List<DrugSpecial>>("specials");
        }
    }

```

To list the specials on your page we need to add some markup. Replace the title from earlier with the following:
```html
<div class="main">
    <ul class="drug-cards">
        @if (specials != null)
        {
            @foreach (var special in specials)
            {
                <li style="background-image: url('@special.ImageUrl')">
                    <div class="drug-info">
                        <span class="title">@special.Name</span>
                        @special.Description
                        <span class="price">@special.GetFormattedBasePrice()</span>
                    </div>
                </li>
            }
        }
    </ul>
</div>

```

If you build and run the application again you should see some default drugs listed with a description and price. Feel free to add new drugs in the `Server` projects `SeedData.cs`. The image can be added to the `Client` projects `wwwroot/img` directory.

Finally we want to add a heading to our web application. In the `Client` project, navigate to `Shared/MainLayout.razor`. Here the main layout for the application is defined. Currently, the only thing here is the body which you have been working on up until now. To add a header with a branding logo and navigation, add the following code:

```html
@inherits LayoutComponentBase

<div class="top-bar">
    <img class="logo" src="img/logo.png" />

    <NavLink href="" class="nav-tab" Match="NavLinkMatch.All">
        <img src="img/medicine.svg" />
        <div>Get Drugs</div>
    </NavLink>
</div>

<div class="content">
    @Body
</div>
```

Build and run the app too see your web store taking form.


### Stage 2
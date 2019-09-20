# BlazorWorkshop

You will see several projects in the repository.

**Workshop.Client** contains all UI components for the application.  
**Workshop.Server** hosts the Blazor app and the backend services.  
**Workshop.Shared** contains shared model types  
**Workshop.ComponentsLibrary** contains helper code that will be used later

## Stage 1

### Adding some drugs

Ensure that the server starts and works properly by building and running it. It should show a white page with the title "Drug Express" 

Open `Pages/Index.razor` in the `Client` project to see the code for the home page. Feel free to change the title and restart the server.  
  
Next, add a `@code` block to the file and initialize a list field to keep track of your specials:

```csharp
@code {
    List<DrugSpecial> specials;
}
```

The `DrugSpecial` type is already defined for you in the `Shared`-project

To fetch the available list of specials we need to call an API on the backend. Blazor provides a preconfigured `HttpClient` through dependency injection that is already set up with the correct base address. You can use the `@inject` directive to inject an `HttpClient` into the component using dependency injection:

```csharp
    @page "/"
    @inject HttpClient HttpClient
```

Override the lifecycle hook `OnInitializedAsync` and use the `GetJsonAsync<T>()` method to get the specials and automatically deserialize the JSON into the `DrugSpecial` type.

```csharp
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

### Adding a heading

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

## Stage 2

### Adding a drug customization dialog

In this section we will look at enabling the users to customize their special before adding it to their order.

We want a customization dialog to pop up when a user clicks a drug special. In Blazor you can attach C# delegates to events where you traditionally would add javascript.  
In the `Client` projects' `Pages/Index.razor`, add the following `@onclick` handler to your existing markup:  

```html
@foreach (var special in specials)
{
    <li @onclick="@(() => Console.WriteLine(special.Name))" style="background-image: url('@special.ImageUrl')">
        <div class="drug-info">
            <span class="title">@special.Name</span>
            @special.Description
            <span class="price">@special.GetFormattedBasePrice()</span>
        </div>
    </li>
}
```

In the `@onclick` attribute you see we added an [expression lambda](https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/statements-expressions-operators/lambda-expressions#expression-lambdas) which prints the special name to the browser console when the user clicks a special.
The `@` symbol is used in Razor files to indicate the start of C# code and parens are used to show where the code begins and ends.

Run the appllication and check the browser developer console for print statements when clicking drug specials.

Printing the name to the console is all well and good, but we want to create a custom handler that shows a dialog for customizing the drug special.
First, add some additional fields next to the list of specials. One of them will hold the `Drug` we are currently configuring and the other will determine whether or not to show the configure dialog.

```csharp
Drug configuringDrug;
bool showingConfigureDialog;
```

Next, add a `ShowConfigureDrugDialog` method to the `@code` block in `Index.razor` for handling when a drug special is clicked.

```csharp
void ShowConfigureDrugDialog(DrugSpecial special)
{
    configuringDrug = new Drug()
    {
        Special = special,
        SpecialId = special.Id,
        Size = Drug.DefaultSize,
    };

    showingConfigureDialog = true;
}

```

We also need to update the `@onclick` handler to call our new method instead of printing to the console.

```html
<li @onclick="@(() => ShowConfigureDrugDialog(special))" style="background-image: url('@special.ImageUrl')">
```

If you run the application now, nothing will happen including printing to the console. This is because we need to add a customization dialog. 
To achieve this we need to create a new component. We have already provided you with a `ConfigureDrugDialog` component found under the `Shared` folder in the `Client` project.
The dialog component needs a `Drug` parameter that specifies the drug being configured and two callbacks `OnCancel` and `OnConfirm`.

Update `Pages/Index.razor` to show the `ConfigureDrugDialog` when a drug special has been selected. The dialog is styled to overlay the page so it does not matter where you put the code block.
The code block will show a `ConfigureDrugDialog` component on the page if the `showingConfigureDialog` is set to true, and it will send the selected drug to the component

```html
@if (showingConfigureDialog)
{
    <ConfigureDrugDialog Drug="configuringDrug" OnCancel="CancelConfigureDrugDialog" OnConfirm="ConfirmDrugDialog" />
}

```

In `Index.razor`, we also need to implement the `CancelConfigureDrugDialog` and `ConfirmDrugDialog` methods that we sent as parameters to the `ConfigureDrugDialog` component. We will leave the `ConfirmDrugDialog` empty for now. 

```csharp
void CancelConfigureDrugDialog() {
    configuringDrug = null;
    showingConfigureDialog = false;
}

void ConfirmDrugDialog() {

}
```

If you start and run the application you should be able to use a slider to decide how many of the item you want, and you should be able to cancel the configuration and go back to the main page.

We want the `OnConfirm` event to add the customized drug to the user's order. Add an `Order` field to the `Index` component. We have already supplied the type `Order`.

```csharp
Order order = new Order();
```

In the `ConfirmDrugDialog` method, add the drug to the order and close the dialog.

```csharp
void ConfirmDrugDialog() {
    order.Drugs.Add(configuringDrug);
    CancelConfigureDrugDialog();
}
```

## Displaying the order

To display the order we have supplied a `ConfiguredDrugItem` component in the `Shared` directory of the `Client` project. 
`ConfiguredDrugItem` takes two parameters, `Drug` for what drug is shown and an event callback `OnRemoved` for when the item is removed from the order.
Add this markup to the `Index.razor` markup just below the main `div`:

```html
<div class="sidebar">
    @if (order.Drugs.Any())
    {
        <div class="order-contents">
            <h2>Your order</h2>

            @foreach (var configuredDrug in order.Drugs)
            {
                <ConfiguredDrugItem Drug="configuredDrug" OnRemoved="@(() => RemoveConfiguredDrug(configuredDrug))" />
            }
        </div>
    }
    else
    {
        <div class="empty-cart">Choose a drug<br>to get started</div>
    }

    <div class="order-total @(order.Drugs.Any() ? "" : "hidden")">
        Total:
        <span class="total-price">@order.GetFormattedTotalPrice()</span>
        <button class="btn btn-warning" disabled="@(order.Drugs.Count == 0)" @onclick="@PlaceOrder">
            Order >
        </button>
    </div>
</div>

```

In the `@code` section you also need to add eventhandlers for the events specified in the markup above. 
In the `RemoveConfiguredDrug` handler we simply remove the drug from the order. In the `PlaceOrder` handler we post the order so its saved in the database and reset the current order. 

```csharp
void RemoveConfiguredDrug(Drug drug)
{
    order.Drugs.Remove(drug);
}

async Task PlaceOrder()
{
    await HttpClient.PostJsonAsync("orders", order);
    order = new Order();
}
```

Run the application and you should now be able to add drugs to your order and see the total in the sidebar to the right.
Pressing the `Order` button will save the order to the database but currently there is nothing in the user interface that indicates it has happened. Check out the next step to find out how to display the user's orders!

## Stage 3

In this stage we want to be able to show the users order status and let them track their order on a map.

### Show order status

To show the order status we need to add a navigation link.
In `Shared/MainLayout.razor` on the client, add a NavLink component after the existing one.

```html
<NavLink href="myorders" class="nav-tab">
    <img src="img/bike.svg" />
    <div>My Orders</div>
</NavLink>
```

Next we need to create the `myorders` component. Create a file called `MyOrders.razor` in the `Pages` directory on the client and add the following code:

```html
@page "/myorders"
<div class="main">
    My orders will go here
</div>
```

Notice how we define the route at the top which corresponds with the `href` attribute on the `NavLink` component in `MainLayout.razor`. This is how Blazor knows which component to load.

Start the application and you should have a new tab at the top. When clicking it you should see the message "*My orders will go here*".

To display a list of orders we again need to inject the http client into our `MyOrders` component and add a code block which requests the data we need and stores it in a local field.

```csharp
@inject HttpClient HttpClient
```

and

```csharp
@code {
    List<OrderWithStatus> ordersWithStatus;

    protected override async Task OnParametersSetAsync()
    {
        ordersWithStatus = await HttpClient.GetJsonAsync<List<OrderWithStatus>>("orders");
    }
}
```

We need to make the UI display different output in different cases:

1. While we're waiting for data to load
2. If it turns out that the user has never placed any orders
3. If the user has placed one or more orders

Let's update the `MyOrders`'s markup inside the main `div` to reflect this:

```csharp
@if (ordersWithStatus == null)
{
    <text>Loading...</text>
}
else if (ordersWithStatus.Count == 0)
{
    <h2>No orders placed</h2>
    <a class="btn btn-success" href="">Order some drugs</a>
}
else
{
    <text>TODO: show orders</text>
}
```

The `<text>` element is not HTML or a component. It is a signal to the compiler that you want to treat the contents within the element as a string and not as C# source code.
Next, delete the database file called `drugs.db` in the `Server` project structure and run the application to show the message that no orders are placed. 

### Showing a grid of orders

Now we want to show all the orders to the user. Replace the `TODO` above with the following code where the component `OrderItem` is given to you.

```html
<div class="list-group orders-list">
    @foreach (var item in ordersWithStatus)
    {
        <OrderItem OrderWithStatus=item />
    }
</div>
```

Feel free to check out or change things on the `OrderItem` component in the `Shared` directory.
Run the application, place an order and go to the `My Orders` tab. You will see your order with status, items, and total.

Next we want to enable the user to see their order details and track their order on a map.

### Order details and tracking

We have provided the more complicated parts of this component.   
Have a look at `Pages/OrderDetails.razor`. This is most of the logic needed to show the order information.
What it does is:

- Hook into the `OnParametersSet` lifecycle method which runs both when the component is instantiated and when the parameters change
- We start polling for updates on the order, cancelling any pending polling for other orders.
- Every 4 seconds, poll the backend for an update to the order.
- If the order is invalid we cancel the polling and prints a message telling the user
- If the order is valid we show when the order is placed and its status.
- Call the `StateHasChanged()` method after polling is complete to tell Blazor to rerender the component. 

Run the application to see the results. Place an order, go to the `My Orders` tab and click track.
We want to show even more on this page, so create a file called `OrderReview.razor` in the `Shared` directory and add the following markup:

```html
@foreach (var Drug in Order.Drugs)
{
    <p>
        <strong>
            @(Drug.Size) x
            @Drug.Special.Name
            ($@Drug.GetFormattedTotalPrice())
        </strong>
    </p>

}

<p>
    <strong>
        Total price:
        $@Order.GetFormattedTotalPrice()
    </strong>
</p>

@code {
    [Parameter] public Order Order { get; set; }
}
```

Back in OrderDetails.razor, replace the TODO with the following code to use your new component:

```html
<div class="track-order-details">
    <OrderReview Order="@orderWithStatus.Order" />
</div>
```

Now run your application to see a functional order details display.
If you are quick enough from ordering, you will see the status change live from *Preparing* to *out for delivery* and finally *delivered* within about one minute.
You can change the delivery time in the `Shared` project's `OrderWithStatus.cs` file by changing the `deliveryDuration` variable. 

Finally we want to add the possibility of tracking the orders on a map. 
To do this we want to use JavaScript Interop which a way of calling browser APIs or existing JavaScript libraries from your blazor code. 
We have supplied you with most of the logic to get this up and running and you mostly have to connect the dots to make it show up in your order details.
The logic can be found in the `ComponentsLibrary` project under `Map`.
In your `_Imports.razor` file, add a using statement to bring the map into scope:

```csharp
@using Workshop.ComponentsLibrary.Map
```

Add the `Map` component to the `OrderDetails` component by adding the following below the `track-order-details` `div`:

```html
<div class="track-order-map">
    <Map Zoom="13" Markers="orderWithStatus.MapMarkers" />
</div>
```

And that's it! Now you have a working drug delivery shop. Check out [Blazor's](https://dotnet.microsoft.com/apps/aspnet/web-apps/blazor) website for more info on the features it provides.

# BlazorWorkshop

## Before you start

Ensure you have the same .NET version installed as this workshop is developed with. This workshop compiles and runs using .NET Core 3.0.0 preview 9 or newer. Download and install both the SDK and the runtime from [.NET Core's downloads page](https://dotnet.microsoft.com/download/dotnet-core/3.0).

### Working from Visual Studio Code

If you're using [Visual Studio Code](https://code.visualstudio.com/) as an editor, it is recommended to use the [C#](https://marketplace.visualstudio.com/items?itemName=ms-vscode.csharp) and [Razor+](https://marketplace.visualstudio.com/items?itemName=austincummings.razor-plus) extensions.

### Working from Visual Studio

If you're using Visual Studio you need enable the preview by going into Tools -> Options -> Environment -> Preview Features and checking the appropriate boxes. Then you can simply open the Workshop solution file in the project root.

## Getting started

In this workshop we will create an online store for ordering pharmaceuticals and having them delivered to us, analogous to food delivery services such as Wolt and Foodora.

As we all know, naming things is one of the hardest problems in computer science. That's why we've come up with a few suggestions for what you can name your online drug store (with a slight mix of Norwegian and English names, in case you want to go international):

- Woltarol
- Paracet Express
- Paralgin Fort
- Uber Drugs
- TakeAway the Pain
- Pillivery
- Dependency Injection Service
- Adams dopkasse
- Mobile Dispensary
- Pharmacuties

This workshop is split into four parts of approximately twenty steps each, let's start Blazoring it! If you get lost along the way or simply want to see the finished product, checkout to the `stage2`, `stage3` or `final` branches to see the next steps or the completed solution.

## Project structure

You will find several projects in this repository.

- **Workshop.Client** contains all UI components for the application. It is compiled as a .NET Standard project because it will be running on the Mono runtime in the browser.
- **Workshop.Server** hosts the Blazor app and the backend services. It will serve the runtime and the client application to the browser.
- **Workshop.Shared** contains shared model types.
- **Workshop.ComponentsLibrary** contains helper code that will be used later.

## Stage 1

### Adding some drugs

Ensure that the server starts and works properly by building and running it. You may need to restore the projects packages before you can launch the server. Any IDE will do this for you, but you can do it manually by running `dotnet restore` in the project root folder.

If you're using Visual Studio (Code) you can press F5, otherwise you can run the server from your terminal with `dotnet run --project Workshop.Server`.

Visiting [`http://localhost:5000`](http://localhost:5000) should take you to an empty page with the title "Insert Business Name Here".

Open [`Pages/Index.razor`](./Workshop.Client/Pages/Index.razor) in the `Client` project to see the code for the home page. Feel free to change the title and restart the server.

Next, add a `@code` block to the file and initialize a list field to keep track of your deals:

```csharp
@code {
    List<DrugDeal> deals;
}
```

The `DrugDeal` type is already defined for you in the `Shared` project.

To fetch the available list of deals we need to call an API on the backend. Blazor provides a preconfigured `HttpClient` through dependency injection that is already set up with the correct base address. You can use the `@inject` directive to inject an `HttpClient` into the component using dependency injection:

```csharp
@page "/"
@inject HttpClient HttpClient
```

Override the lifecycle hook `OnInitializedAsync` and use the `GetJsonAsync<T>()` method to get the deals and automatically deserialize the JSON into the `DrugDeal` type.

```csharp
@code {
  List<DrugDeal> deals;

  protected async override Task OnInitializedAsync()
  {
    deals = await HttpClient.GetJsonAsync<List<DrugDeal>>("deals");
  }
}
```

To list the deals on your page we need to add some markup. Replace the title from earlier with the following:

```html
<div class="main">
  <ul class="drug-cards">
    @if (deals != null) { @foreach (var deal in deals) {
    <li style="background-image: url('@deal.ImageUrl')">
      <div class="drug-info">
        <span class="title">@deal.Name</span>
        @deal.Description
        <span class="price">@deal.GetFormattedBasePrice()</span>
      </div>
    </li>
    } }
  </ul>
</div>
```

If you build and run the application again you should see some a list of drugs along with their descriptions and prices. Feel free to add new drugs in [`SeedData.cs`](./Workshop.Server/SeedData.cs), but you need to drop the database to force it to re-seed. New images can be added to the `Client` projects `wwwroot/img` directory.

### Adding a navigation bar

We would like to add some structure to the layout of our web application, and we'll start with a navigation bar. In the `Client` project, navigate to [`Shared/MainLayout.razor`](./Workshop.Client/Shared/MainLayout.razor). This is where the main layout for the web application is defined. Currently the only thing it contains is the body which you have been working on up until now. To add a header with a branding logo and navigation, replace its content with the following code:

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

Build and run the app to see your web store taking shape.

## Stage 2

### Adding a drug customization dialog

In this section we will look at enabling the users to customize their deal before adding it to their order.

We want a customization dialog to pop up when a user clicks a drug deal. In Blazor you can attach C# delegates to events where you traditionally would add javascript.
In the `Client` projects' [`Pages/Index.razor`](./Workshop.Client/Pages/Index.razor), add the following `@onclick` handler to your existing markup:

```html
@foreach (var deal in deals) {
<li
  @onclick="@(() => Console.WriteLine(deal.Name))"
  style="background-image: url('@deal.ImageUrl')"
>
  <div class="drug-info">
    <span class="title">@deal.Name</span>
    @deal.Description
    <span class="price">@deal.GetFormattedBasePrice()</span>
  </div>
</li>
}
```

In the `@onclick` attribute you see we added an [expression lambda](https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/statements-expressions-operators/lambda-expressions#expression-lambdas) which prints the deal name to the browser console when the user clicks a deal.
The `@` symbol is used in Razor files to indicate the start of C# code and parens are used to show where the code begins and ends.

Run the application and check the browser developer console for print statements when clicking drug deals.

Printing the name to the console is all well and good, but we want to create a custom handler that shows a dialog for customizing the drug deal.
First, add some additional fields next to the list of deals. One of them will hold the `Drug` we are currently configuring and the other will determine whether or not to show the configure dialog.

```csharp
Drug configuringDrug;
bool showingConfigureDialog;
```

Next, add a `ShowConfigureDrugDialog` method to the `@code` block in `Index.razor` for handling when a drug deal is clicked.

```csharp
void ShowConfigureDrugDialog(DrugDeal deal)
{
  configuringDrug = new Drug()
  {
    Deal = deal,
    DealId = deal.Id,
    Quantity = Drug.DefaultQuantity,
  };

  showingConfigureDialog = true;
}

```

We also need to update the `@onclick` handler to call our new method instead of printing to the console.

```html
<li
  @onclick="@(() => ShowConfigureDrugDialog(deal))"
  style="background-image: url('@deal.ImageUrl')"
></li>
```

If you run the application now, nothing will happen including printing to the console. This is because we need to add a customization dialog.
To achieve this we need to create a new component. We have already provided you with a `ConfigureDrugDialog` component found under the `Shared` folder in the `Client` project.
The dialog component needs a `Drug` parameter that specifies the drug being configured and two callbacks `OnCancel` and `OnConfirm`.

Update [`Pages/Index.razor`](./Workshop.Client/Pages/Index.razor) to show the `ConfigureDrugDialog` when a drug deal has been selected. The dialog is styled to overlay the page so it does not matter where you put the code block.
The code block will show a `ConfigureDrugDialog` component on the page if the `showingConfigureDialog` is set to true, and it will send the selected drug to the component

```html
@if (showingConfigureDialog) {
<ConfigureDrugDialog
  Drug="configuringDrug"
  OnCancel="CancelConfigureDrugDialog"
  OnConfirm="ConfirmDrugDialog"
/>
}
```

In [`Pages/Index.razor`](./Workshop.Client/Pages/Index.razor), we also need to implement the `CancelConfigureDrugDialog` and `ConfirmDrugDialog` methods that we sent as parameters to the `ConfigureDrugDialog` component. We will leave the `ConfirmDrugDialog` empty for now.

```csharp
void CancelConfigureDrugDialog() {
  configuringDrug = null;
  showingConfigureDialog = false;
}

void ConfirmDrugDialog() {
}
```

If you run the application you should be able to select a drug and use a slider to indicate the quantity you'd like to order, and you should be able to cancel the configuration and go back to the main page.

We want the `OnConfirm` event to add the drug to the user's order. Add an `Order` field to the `Index` component. We have already supplied the type `Order`.

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
Add this markup to the [`Index.razor`](./Workshop.Client/Pages/Index.razor) markup just below the main `div`:

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

In the `@code` section you also need to add event handlers for the events specified in the markup above.
In the `RemoveConfiguredDrug` handler we simply remove the drug from the order. In the `PlaceOrder` handler we post the order so it's saved to the database and reset the current order.

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
Clicking the `Order` button will save the order to the database but currently there is nothing in the user interface that indicates it has happened. Check out the next step to find out how to display the user's orders!

## Stage 3

In this stage we want to be able to show the users order status and let them track their order on a map.

### Show order status

To show the order status we need to add a navigation link.
In [`Shared/MainLayout.razor`](./Workshop.Client/Shared/MainLayout.razor) on the client, add a NavLink component after the existing one.

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

Start the application and you should have a new tab at the top. When clicking it you should see the message "_My orders will go here_".

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
  @foreach (var item in ordersWithStatus) {
  <OrderItem OrderWithStatus="item" />
  }
</div>
```

Feel free to check out or change things on the `OrderItem` component in the `Shared` directory.
Run the application, place an order and go to the `My Orders` tab. You will see your order with status, items, and total.

Next we want to enable the user to see their order details and track their order on a map.

### Order details and tracking

We have provided the more complicated parts of this component.
Have a look at [`Pages/OrderDetails.razor`](./Workshop.Client/Pages/OrderDetails.razor). This is most of the logic needed to show the order information.

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
@foreach (var Drug in Order.Drugs) {
<p>
  <strong>
    @(Drug.Quantity) x @Drug.Deal.Name ($@Drug.GetFormattedTotalPrice())
  </strong>
</p>
}

<p>
  <strong>
    Total price: $@Order.GetFormattedTotalPrice()
  </strong>
</p>

@code { [Parameter] public Order Order { get; set; } }
```

Back in OrderDetails.razor, replace the TODO with the following code to use your new component:

```html
<div class="track-order-details">
  <OrderReview Order="@orderWithStatus.Order" />
</div>
```

Now run your application to see a functional order details display.
If you are quick enough from ordering, you will see the status change live from _preparing_ to _out for delivery_ and finally _delivered_ within about one minute.
You can change the delivery time in the `Shared` projects [`OrderWithStatus.cs`](./Workshop.Shared/OrderWithStatus.cs) file by changing the `deliveryDuration` variable.

Finally we want to let users track their orders on a map.
To do this we want to use JavaScript Interop which a way of calling browser APIs or existing JavaScript libraries from your blazor code.
We have supplied you with most of the logic to get this up and running and you mostly have to connect the dots to make it show up in your order details.
The logic can be found in the `ComponentsLibrary` project under `Map`.
In your [`_Imports.razor`](./Workshop.Client/_Imports.razor) file, add a using statement to bring the map into scope:

```csharp
@using Workshop.ComponentsLibrary.Map
```

Add the `Map` component to the `OrderDetails` component by adding the following below the `track-order-details` `div`:

```html
<div class="track-order-map">
  <Map Zoom="13" Markers="orderWithStatus.MapMarkers" />
</div>
```

And that's it! Now you have a working drug delivery service. Check out [Blazor's](https://dotnet.microsoft.com/apps/aspnet/web-apps/blazor) website for more info on the features it provides.

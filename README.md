# UBB-SE-2025-MarketMinds

A Windows application for managing an online marketplace with features for buying, selling, borrowing, and auctioning products.

## Features

- 🛍️ **Product Management**
  - Buying products
  - Selling products
  - Borrowing products
  - Auction system
- 🔍 **Search & Filter**
  - Product filtering by tags
  - Advanced search functionality
- 📊 **User Features**
  - Product reviews
  - Product comparison
  - Shopping basket
  - Bidding system

## Dev Setup

Create an `appsettings.json` file in the `MarketMinds` directory with the following content:

```json
{
  "LocalDataSource": "np:\\\\.\\pipe\\your_pipe_name\\tsql\\query",
  "InitialCatalog": "your_database_name",
  "ImgurSettings": {
    "ClientId": "your_client_id",
    "ClientSecret": "your_client_secret"
  }
}
```

You can find the pipe by running:
```cmd
SqlLocalDB.exe start
SqlLocalDB.exe info MSSQLLocalDB
```

## Project Structure

```
UBB-SE-2025-MarketMinds/
├── 📱 MarketMinds/                   # Main application directory
│   ├── 🖼️ Views/                     # UI components (View layer)
│   │   └── Pages/                    # Main application pages
│   │
│   ├── 📊 ViewModels/                # View models (ViewModel layer)
│   │
│   ├── 📦 Models/                    # Data models (Model layer)
│   │
│   ├── 🔄 Services/                  # Business logic services
│   │
│   ├── 🗄️ Repositories/              # Data access layer
│   │
│   ├── 🧰 Helpers/                   # Helper utilities
│   │
│   ├── 🗄️ Data/                      # Data layer
│   │
│   ├── 🖼️ Assets/                    # Application assets
│   │
│   ├── ⚙️ Properties/                # Project properties
│   │
│   ├── 📄 App.xaml                   # Application definition
│   ├── 📄 App.xaml.cs                # Application code-behind
│   ├── 📄 MainWindow.xaml            # Main window definition
│   ├── 📄 MainWindow.xaml.cs         # Main window code-behind
│   ├── 📄 MarketMinds.csproj         # Project file
│   ├── 📄 MarketMinds.sln            # Solution file
│   └── 📄 appsettings.json           # Application settings
```
## Demo

Hand made GUI video - Seminar 2: [Watch on YouTube](https://youtu.be/OBPRiNfDDVs?si=lRMacDvDzZtjhuQG)

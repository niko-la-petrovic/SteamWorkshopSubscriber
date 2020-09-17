# Steam Workshop Subscriber

A cross-platform Selenium-based .NET Core solution for automating subscriptions to Steam Workshop items. 

# Latest Notes
- For some reason, the Firefox driver seems to be very slow at obtaining references to indiviual DOM elements.

# How to use as is

### Download the binaries 
TLDR: You can find them here [here](https://github.com/niko-la-petrovic/SteamWorkshopSubscriber/releases).

 The binaries will also contain the **workshop_list.json** file.
### Configuring
The **workshop_list.json** file contains a JSON string array of Steam Workshop item urls. You can, retaining the existing format, create your own lists and share them with others.

### Running
On Windows, just run the SteamWorkshopSubscriber.exe, or on other platforms run the dll named the same way with dotnet CL utility.

### Building

To build the project yourself, you require the one of the .NET Core runtimes.

---

# Motivation for making the project

A friend was lazy to make a Steam collection of workshop items and instead wrote a list of 200 mods. Thanks for the idea for the project, Volu!


# Extending the project

You can add Selenium-supported web drivers into the appropriate folders and change the enums and add to the SelectBrowser and SelectBrowserVersion methods of the SteamWorkshopSubscriber class.

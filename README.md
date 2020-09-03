# StashEdit
First time making a github repo. This was done using C# WPF.


1. This is a Third-Party extension of StashApp and makes things easier to manage your porn collection.
2. I may update the readme and add some images explaining the details
3. What it can do so far...

# Setup
### Requires .NET Core Runtime [here](https://dotnet.microsoft.com/download/dotnet-core/current/runtime)
* Download exe from [here](https://github.com/Mfkisdo/StashEdit/releases) or clone the repo in Visual Studio. For the moment it only works on Windows.
* XML Settings under options.
    * If you use stash_porndb_scrapper, I would suggest creating a new tag in stash 'scan'.
    * Enter the path to your stash db wherever it may be.
    * API Limit is default to 50 usually, this effects the amount of matches to a scene is shown. The max is 100.
    * Naming style only allows the current 3 "Publisher;Actor;Title" but can be switched around to whatever you prefer. Make sure they have ; between them.
    * Images to Tags, This will import a folder that you have with images. The image and tag name must be the same but saves a lot of time.
    * Sort Folder is the holding point before stash gets it and allows you rename before it reaches stash. 
    * Under database you can clear everything with the 'scan' tag
    * Destination folders, still working on this. Though this is a quaility of life change. Move files from the sort folder with folder names in the destination folder. This
        will allow multiple folders. 
        * Filename in sort folder: 'C:\Sort\hardx - actor name - title'
        * Destination folders: C:\Actors;C:\Studios
        * It will first search actors folder and match the actor name and move the file there, if not it will search the studio folder and so.

# Abilities

![image](https://github.com/Mfkisdo/StashEdit/blob/master/StashEdit/Images/Example1.jpg)
    
* Allows you to search the stash db for a specific file name/recently added/no URL attached to a scene/sort folder (set up from settings)
* A List of results will be shown as file names and then search the pornDB for similiar names you can edit the textbox for different results, if there is more than one result it   * will display all found url's with images to help selected the right scene data.
* If you find a matching scene on PornDB select the image and a new name and title will be generated automatically.
* What saving does it renames the file and also updates the title/url/details/studio but not the image to the stash app db. Updated scenes are also tagged with 'scan'
   * From here you can search stash with the scan tag and update the scene manually using a built in scraper
   * If you set up stash_porndb_scrapper, then just use database -> StartPDRename, then click 'Delete Tag (scan). Which will remove all scenes with the scan tag for you.
* Double Click file name in list to open it in default video player.

Off note: Still trying to figure out git with Visual Studio.

[Wiki](https://github.com/Mfkisdo/StashEdit/wiki)

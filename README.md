# StashEdit
First time making a github repos. This was done using C# WPF.


1. This is a Third-Party extension of StashApp and makes things easier to manage your porn collection.
2. I may update the readme and add some images explaining the details
3. What it can do so far...

# Setup
1. Under the settings a tab with XML Settings is really the only that can change at the moment.
    * Create a new tag called 'scan' in Stash
    * Enter the path to your stash db wherever it may be.
    * API Limit is default to 50 usually, this effects the amount of matches to a scene is shown. The max is 100.
    * Naming style only allows the current 3 contents but can be switched around if you prefer. Make sure they have ; between them.
    * Images to Tags, This will import a folder that you have with images with tag names. The image and tag name must be the same but saves a lot of time.
    * Sort Folder is the holding point before stash gets it and allows you rename before it reaches stash. 
        * This mainly is for the porndb scraper with Python found [here](https://github.com/pierre-delecto/stash_theporndb_scraper). 
        * There is also another benefit of having this because this program will run it. Remember that this program on scene update will apply 'scan' tag to whatever it changes            and when the scrapper is ran it will only search for tags matching 'scan'. This will import image/url/tags/studio/
            * Under database you can clear everything with the 'scan' tag
    * Destination folders, still working on this. Though this is a quaility of life change. Move files from the sort folder with folder names in the destination folder. This
        allow multiple folders. 
        * Filename in sort folder: 'C:\Sort\hardx - actor name - title'
        * Destination folders: C:\Actors;C:\Studios
        * It will first search actors folder and match the actor name and move the file there, if not it will search the studio folder and so.

# Abilities

![image](https://github.com/Mfkisdo/StashEdit/blob/master/StashEdit/Images/Example1.jpg)
    
1. Allows you to search the stash db for a specific file name/recently added/no URL attached to a scene/sort folder set up from settings
2. A List of results will be shown as file names and then search the pornDB for similiar names, if there is more than one result it will display all found url's with images to help selected
  the right scene data.
3. If you find a matching scene on PornDB you can save.
4. What saving does it renames the file and also updates the title/url/details/studio to the stash app db. Updated scenes are also tagged with 'scan'
5. From here you have two options
    i. I have added support 

Warning this is still under development and may take some time to work the kinks out.

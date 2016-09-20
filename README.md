# SyncMedia
Copy pictures and video from source folder to destination folder tree and prevents dupes by hashing each file and saving
a xml list of hashes.

Once the application starts you will need to "set" three folders (This will be saved).
The first is the folder you downloaded your files from your camera or phone to. 
This folder needs to be outside of the Destination folder or it will make a mess.

For example you shouldn't set the source and destination folder like c:\pictures\ and c:\pictures\sorted.
The source folder and all sub folders will be imported.

A good example would be c:\pictures\ and c:\users\johndoe\my pictures\

The destination folder should be set to an empty folder the first time you use the application.

The reject duplicate folder should also be a new empty folder outside of the source folder.

A good example would be to create a folder in your destination folder. c:\users\johndoe\my pictures\rejects\

If you are syncing multiple devices when you import them give them unique names.
For example when Jane imports her iPhone she labels them "Jane iPhone - date-time".
The "Update Naming List" will search the source folder for possible naming conventions.
Check the box next to the names you want to retained on the files after they are imported.
If none are selected all the files will be named in the following way:

For pictures: Date Taken - sequence number from this import (a number from 001 to the total count of files you are importing) 
For movies: Date Modified - sequence number from this import (a number from 001 to the total count of files you are importing)

Once all of your folders are set and any names checked you can press "Sync Media" and the gray area will 
populate with the file names and a status message for each.

Each file is hashed to get a unique signature so the system will not place duplicate files into the folder structure.

The application will only sort the following file types:
.jpg, .png, .bmp, .jpeg, .gif, .tif, .tiff, .mov, .mp4, .wmv, .avi, .m4v, .mpg and .mpeg

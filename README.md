# CSharp-Pluralsight-Downloader
A Pluralsight Downloader using C#

The motive behind this project:
-----------------------------
- I have a valid subscription to pluralsight and due to internet problems and not having the money luxury to renew my subsrciptin, I wanted to save the videos locally to that I could be able to watch them later.
- Display the number of ***raters*** of a given course which is not displayed on Pluralsight website. 

The idea of the project:
-----------------------------
Simply trying to mimic ***Internet Download Manager(IDM)***, through fiddler and chrome network panel.
- At the end, the video page require a valid video url to display it.
- The video url is coming through simple API call but with some parameters related to the which clip video to return and logged user cookie(for un-free videos).
- After inspection, I found that the url parameters are included in the video clip data(which is also coming though simple API call), only the authentication cookie require more work.

Things to note:
-----------------------------
 - Your feedback is highly appreciated.
 - The project may be not fully *well* *right* *configurable*, but it does the job very ***well*** and ***enough***.
 - The project is meant to run locally for simplicty(instead of save videos on the server then save them again on the user PC), debugging and extendability.
 - Please please, don't forget to replace the ***USER_NAME*** and ***PASSWORD*** located in ***Web.config*** with your subscription values, as well as ***DOWNLOAD_FOLDER_PATH*** with your desired folder to download in.
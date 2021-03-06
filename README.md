# LEdit (BETA)

Version Information: VB 1.7 | S4A
Ready to be used!
(you need to compile the server itself)
</h3>

---

## Updates

Please wait a around a week to see new features and things being added to LEdit, this is because we recently sorted out our Software Team, and now are briefing them. In addition, Shift is undergoing some drastic changes to our documents and to the way things work, so pease bear with us.
The current version works :)

## What is this?

LEdit is an app designed for teams to work off of a single shared server (with all changes syncing over to anyone with the application open - to prevent any conflicts in peoples work). How this will work is you will configure your team directory on the application, and then everything in that directory will be shared with your team members you setup accounts for.

## LEdit Versions V7 & Onwards (Excluding LEdit_V1_Client)

We've made a **huge** change from using the normal .NET framework to the .NET Core framework, this was to allow us to have things cross-platform, but as it's now cross-platform, there is a lot more to manage with the features. V1.7s are early stages of a better LEdit, so please hang on with us as there will be issues - for example, the settings are **very** slow to respond, and don't fully work yet.
If you are using Windows, **currently**, LEdit_V1_Client is the most stable to use, however, since this last push, we are **discontinuing** *LEdit_V1_Client*, to work solely on *LEdit_Client*, with all of our client updates going to *LEdit_Client* now.

## FAQ

**Why isn't this or that here already!?**

Remember, this is a **Beta**. Currently things are still being built

**Why can't I just use FTP to collaborate with my team?**

You can, the thing is; you can but only one of you can work on something at a time. Multi-sided live editing is not a feature right now. 

This will let you see what others are working on, make suggestions, talk you through solutions, and see versions/revert back. We also automatically back up your data, so if anything goes down or breaks you can move to a temp server and get back to normal seemlessly!

## Development / Testing

This is still a WIP, so treat it as such.

### Bugs

It would be appreciated if you could alert us to bugs, it'd help to include information as to the exact situation that caused the bug; code lines, things you did, etc. Reproducing/fixing the bug is a priority. A good way to do this is through [isues](https://github.com/Shift-Development/LEdit/issues/new).

### IDE Usage

* Visual Studio Code
   * Tested and is known to work effectively
   
* Sublime Text
  * Tested with a few issues;
     * Alerts you when a change has been made to an open file
     * No comparison feature to compare with other versions of the file
 
### Requirements

1. A web server (To host the API)

2. A windows server (To host the app)

3. Enough knowledge of the code to work with the codebase

### Setup

Note: In the future this will be an easier process, currently you'll need to manually compile the app.

1. Open the server Visual Studio Project and edit "program.cs"
  * Configure everything in the Settings Namespace to how you would like it
  * Be sure to add in your server IP

2. Get your developers to download the client application

3. Use the user account creator (www/registration/reg.php) to register any developers that you'd like to have access onto the app

4. Import the db.sql (sql/db.sql) file into your database

5. Upload the web server files to your web server and configure your database and directory information
  * The web server should **ONLY** allow connections from the server's IP address, if it isn't then you **WILL** be **MASSIVELY** vulnerable.
  
 ## Setting up the client application
 1. Open it
 2. Type in Settings (yes with the capital S)
 3. Configure the settings to your needs :)

#### Adding Live Changes

To enable live changes, open your IDE settings and enable autosave.

Now for the delay, we personally recommend a delay of <b>250ms</b> as that syncs roughly every word (According to our tests, however, it would be dependant on your typing) and should cause less sync delay. If you reduce the auto save delay beyond our limit it's very possible that it could increase the delay in reaching the server, which would increase the delay for your team to get changes which would be counter-productive.

We are trying to speed everything up so people can reduce any delays.

## Planned Features
 https://github.com/Shift-Development/LEdit/projects/1
 
# A run down of some of the terms we use with LEdit
 ## The Stages
  S1 -> Initial release<br>
  S2 -> Stable release<br>
  S3 -> BETA release<br>
  S4 -> Cross-platform release<br> 
  - S4A -> Client cross-platform release
  - S4B -> Server cross platform release<br>
    
 ## Points of release
  A -> ALPHA<br>
  B -> BETA<br>
  P -> PUBLIC (the final point)

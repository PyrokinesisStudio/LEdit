# LEdit (BETA)

Version Information: VB 1.6.1 | S3</h3>

---

## What is this?

LEdit will allow teams to live share their code with other members and edit it. How this will work is you will configure your team directory on the application, and then everything in that directory will be shared with your team members you setup accounts for.

## FAQ

**Why isn't this or that here already!?**

Remember, this is a **Beta**. Currently things are still being built, like;

* User account creator - BCrypt (.NET -> CryptSharp Library, or PHP)

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

2. Configure the client application to how you would like it
  * Be sure to remember to add in your server IP 

3. Create your user accounts **(not currently possible)**
  * A tool to create those will be coming very soon

4. Import the setup.sql file into your database

5. Upload the web server files to your web server and configure your database and directory information
  * The web server should **ONLY** allow connections from the server's IP address, if it isn't then you **WILL** be **MASSIVELY** vulnerable.

#### Adding Live Changes

To enable live changes, open your IDE settings and enable autosave.

Now for the delay, we personally recommend a delay of <b>250ms</b> as that syncs roughly every word (According to our tests, however, it would be dependant on your typing) and should cause less sync delay. If you reduce the auto save delay beyond our limit it's very possible that it could increase the delay in reaching the server, which would increase the delay for your team to get changes which would be counter-productive.

We are trying to speed everything up so people can reduce any delays.

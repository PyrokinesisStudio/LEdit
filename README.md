# LEdit (BETA)

Version Information: VB 1.6.1 | S3</h3>

---

## What is this?

LEdit will allow teams to live share their code with other members and edit it. How this will work is you will configure your team directory on the application, and then everything in that directory will be shared with your team members you setup accounts for.

#### Why isn't this or that here already!?

Remember, this is a **Beta**. Currently things are still being built, like;

* User account creator - BCrypt (.NET -> CryptSharp Library, or PHP)

## FAQ

<h3>Why can't I just use FTP to collaberate with my team? [NOTE: These features will be ready for when LEdit is ready]</h3>
Well, you can but, for example, if two of you are working on the same file trying to do something, only one of you can actually work on the file at one time, because if person x uploads their progress, then you upload yours, person x's progress gets lost because you would've uploaded an older copy of the work with your edits in.

LEdit also allows for your peers to be able to see your work while you do it as well as help you if you are stuck on something, and they can walk you through a solution, which would be far more difficult if they did the whole thing then talked to you about it as some people just don't learn like that.

LEdit also tracks what people do and so if somebody needs to revert back to a previous version of a file, then they can do, or if you have a rogue team mate then you can track that team mate down and again, revert back.

Finally, LEdit automatically backs up your data so if there's a problem, and your server goes down, you can take your backup, move it onto a temporary server and use LEdit again, as normal, then once your main server is back online, you can just transfer the data over again very simply.

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

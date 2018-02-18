---
layout: default
---
# [](#header-1)LEdit - this page is out of date, view our github repo and view readme.md

# [](#header-2)Information
**Version Information**: VA 1.1 [Version Alpha 1.1] | P2 [Phase 2] <br>
**ALPHA**<br>
_A user account tool is on it's way (hopefully, one should be up in a few days), but for now there is no way_
_This should hopefully come out of alpha relatively soon_

# [](#header-2)Our IDE Recommendation
 Visual Studio Code 
 
# [](#header-2)Tested IDEs
Visual Studio Code<br>
&nbsp-> Works perfectly<br>
Sublime Text<br>
&nbsp-> Alerts you when a change has been made to an open file<br>
&nbsp-> No comparison feature to compare with other versions of the file 

# [](#header-2)Bugs
We would **REALLY** appreciate it if you guys alerted us immediately about any bugs and it would help tremendously if you guys could tell us accurately how you came to this bug 

# [](#header-2)What is LEdit?
LEdit is a piece of software, which allows teams to live share their code to other members and also just edit it normally, so what will happen is that you will configure your team directory on the server application and everything in that directory will be shared to your other members (who you have to create an account for) 

# [](#header-2)Why can't I just use FTP to collaberate with my team? 
_[NOTE: Some features may not be ready]_<br>
Well, you can but, for example, if two of you are working on the same file trying to do something, only one of you can actually work on the file at one time, because if person x uploads their progress, then you upload yours, person x's progress gets lost because you would've uploaded an older copy of the work with your edits in.<br>

LEdit also allows for your peers to be able to see your work while you do it as well as help you if you are stuck on something, and they can walk you through a solution, which would be far more difficult if they did the whole thing then talked to you about it as some people just don't learn like that.<br>

LEdit also tracks what people do and so if somebody needs to revert back to a previous version of a file, then they can do, or if you have a rogue team mate then you can track that team mate down and again, revert back.<br>

Finally, LEdit automatically backs up your data so if there's a problem, and your server goes down, you can take your backup, move it onto a temporary server and use LEdit again, as normal, then once your main server is back online, you can just transfer the data over again very simply.

# [](#header-2)Requirements
1. A web server _[To host the API]_

2. A windows server _[To host the app]_

# [](#header-2)Setting Up LEdit
_[An easier way to set things up is in the works, but for now you will have to manually compile the app]_

1. Open the server Visual Studio Project and edit "program.cs" -> Configure everything in the Settings Namespace to how you would like it -> Be sure to add in your server IP

2. Configure the client application to how you would like it -> Be sure to remember to add in your server IP

3. Create your user accounts (right now you can't) -> A tool to create those will be coming very soon

4. Import the setup.sql file into your database

5. Upload the web server files to your web server and configure your database and directory information -> THE WEB SERVER SHOULD ONLY allow connections from the server's IP address -> If it isn't then you **WILL** be **MASSIVELY VULNERABLE**

# [](#header-2)Usage
Log in by typing: <br>
> ```RunLogin {Username} {Password}```<br>
 EXAMPLE: <br>
>  ```RunLogin Test_Account MyAwesomeProtectivePassword!x1x!1x!``` <br>

Now any actions you need to perform, you just minimize the app and you do them within the folder that was chosen in My Documents 

[ThisHosting.Rocks](https://thishosting.rocks) - https://thishosting.rocks <br>
[IceLine Hosting](https://iceline-hosting.com) - https://iceline-hosting.com 

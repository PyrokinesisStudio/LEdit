<h1>LEdit (VA)<h1>

<hr />
<h3>Version Information: VA 1.1 | S2</h3>
<h3>This is an <b>alpha</b> version!</h3>
<hr />

<h3>Our IDE Recommendation<h3>
<u>Visual Studio Code</u>

<h3>Tested IDEs<h3>
Visual Studio Code
 -> Works perfectly
Sublime Text
 -> Alerts you when a change has been made to an open file
 -> No comparison feature to compare with other versions of the file

<h3>Bugs!</h3>
We would <b>REALLY</b> appreciate it if you guys alerted us immediately about any bugs and it would help tremendously if you guys could tell us accurately how you came to this bug

<h3>What is LEdit?</h3>
LEdit is a piece of software, which allows teams to live share their code to other members and also just edit it normally, so what will happen is that you will configure your team directory on the server application and everything in that directory will be shared to your other members (who you have to create an account for)

<h3>Why can't I just use FTP to collaberate with my team? [NOTE: These features will be ready for when LEdit is ready]</h3>
Well, you can but, for example, if two of you are working on the same file trying to do something, only one of you can actually work on the file at one time, because if person x uploads their progress, then you upload yours, person x's progress gets lost because you would've uploaded an older copy of the work with your edits in.

LEdit also allows for your peers to be able to see your work while you do it as well as help you if you are stuck on something, and they can walk you through a solution, which would be far more difficult if they did the whole thing then talked to you about it as some people just don't learn like that.

LEdit also tracks what people do and so if somebody needs to revert back to a previous version of a file, then they can do, or if you have a rogue team mate then you can track that team mate down and again, revert back.

Finally, LEdit automatically backs up your data so if there's a problem, and your server goes down, you can take your backup, move it onto a temporary server and use LEdit again, as normal, then once your main server is back online, you can just transfer the data over again very simply.

<h3>Requirements</h3>
1. A web server
 -> To host the API

2. A windows server
 -> To host the app

<h3>Setting Up LEdit</h3>
[An easier way to set things up will be done in future, but for now you will have to manually compile the app]

1. Open the server Visual Studio Project and edit "program.cs"
 -> Configure everything in the Settings Namespace to how you would like it
  -> Be sure to add in your server IP

2. Configure the client application to how you would like it
 -> Be sure to remember to add in your server IP 

3. Create your user accounts (right now you can't)
 -> A tool to create those will be coming very soon

4. Import the setup.sql file into your database

5. Upload the web server files to your web server and configure your database and directory information
 -> THE WEB SERVER SHOULD <b>ONLY</b> allow connections from the server's IP address
  -> If it isn't then you <b>WILL</b> be <b>MASSIVELY</b> vulnerable

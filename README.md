<h1>LEdit<h1>

<hr />

<h3>All connections aren't through an SSL so they're vulnerable (at least I think thats how it is)</h3>
<h3>...So I wouldn't recommend using this until I take this text out of the README</h3>
<hr />

<h3>What is LEdit?</h3>
LEdit is a piece of software, which allows teams to live share their code to other members and also just edit it normally, so what will happen is that you will configure your team directory on the server application and everything in that directory will be shared to your other members (who you have to create an account for)

<h3>Why can't I just use FTP to collaberate with my team? [NOTE: These features will be ready for when LEdit is ready]</h3>
Well, you can but, for example, if two of you are working on the same file trying to do something, only one of you can actually work on the file at one time, because if person x uploads their progress, then you upload yours, person x's progress gets lost because you would've uploaded an older copy of the work with your edits in.

LEdit also allows for your peers to be able to see your work while you do it as well as help you if you are stuck on something, and they can walk you through a solution, which would be far more difficult if they did the whole thing then talked to you about it as some people just don't learn like that.

LEdit also tracks what people do and so if somebody needs to revert back to a previous version of a file, then they can do, or if you have a rogue team mate then you can track that team mate down and again, revert back.

Finally, LEdit automatically backs up your data so if there's a problem, and your server goes down, you can take your backup, move it onto a temporary server and use LEdit again, as normal, then once your main server is back online, you can just transfer the data over again very simply.

<h3>Setting Up LEdit [NOTE: LEdit is not ready yet]</h3>
[An easier way to set things up will be done in future, but for now you will have to manually compile the app]

1. Open the server Visual Studio Project and edit "program.cs"
 -> Configure everything in the Settings NameSpace (so far, the classes MySQL_Config and Socket_Config)

2. Import the database file into your database (I haven't uploaded it yet - I will do within a few weeks)

3. Create your user accounts (right now you can't as all passwords are hashed in BCrypt)

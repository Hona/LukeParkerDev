+++ 
date: "2021-01-07"
title: "Why I switched to Rider from Visual Studio for C# Development"
slug: "why-I-switched-to-Rider-from-vs-for-csharp-development"
tags: ["Rider", "Visual Studio", "C#"]
series: []
hook: "An overview of what drove me away from Visual Studio - and my first impressions of Rider."
+++

## Context

I have personally been using Visual Studio since I started learning to code, all the way back in 2010 using VB.NET. Over that time I have grown more comfortable with using the IDE. Made by Microsoft, and considered the flagship way to develop for C#, why would I want to switch away from it?

## Visual Studio's Shortcomings

* Very laggy - Visual Studio as a piece of software is an age-old behemoth, an unavoidable recipe for lag, and severe slow downs. This becomes evident once you start loading projects larger than a simple proof of concept. More recently, I've noticed these issues becoming more extreme.
* Many, many slight issues and bugs. Little annoying issues that aren't code destroying, but enough to slow down your productivity, or break you out of your flow state. An example of one of the bugs is an unlimited loading time when adding a reference to another project through a *'quick' action*.
* The *almost* necessity for ReSharper, and other VS Extensions.

Although my list isn't that large, they all contribute to being less productive, and in ReSharper's case, already reliant on JetBrain's products.

## Making the Switch

> It should be mentioned that Rider is a paid product, however you can get a free for personal use license as a student, or even as an open source maintainer.

Downloading Rider was a simple process through their website, and the install process was seamless.

A nice feature that comes with Rider (and ReSharper for VS), was the keymap options, which allows you to instantly get a familiar experience, no matter what IDE you come from.

## Initial Impressions

I immediately loaded up a reasonably large sized project (10+ projects), to see how Rider holds up. Wow! I was blown away... I've never seen such a smooth experience; loading a solution, indexing files/symbols, code inspection + analysis were all amazingly smooth! The background tasks that usually run in the 'background' of Visual Studio - often causing the syntax highlighting to break, IntelliSense to freeze, and sometimes the window itself to stop responding - went by unnoticed in Rider. You can start coding right away as these tasks fire off in the background, so if you boot up Rider with that bug fix that suddenly came to you in the shower, no need to wait upwards of several minutes to open the IDE. 

## Rider's Downfalls

Even though Rider is a great product, and I will not be going back to Visual Studio anytime soon, it still comes with its downsides.

* No way to auto-generate a Dockerfile - in VS you can `Right Click -> Add -> Docker Support` to instantly dockerize your project, however this option does not exist in Rider. This is the only reason that I open Visual Studio anymore, to simply generate Dockerfiles.
* Some code formatting issues, upon saving rider keeps pointless whitespace, so if you have accidental spaces after a `;` or `}`, it will be there, luckily git diffs show this so I can manually delete them before commiting, but this is a missed feature from VS. Autoformatting does fix this, but really, it should be done on save.
* You cannot edit a `.csproj` file without unloading the project first, making the change, and reloading it. Coming from VS, you know that you can simply click the project and edit the file without issue.

## Final Thoughts

Rider is a great product, and the simple fact that it is **significantly more performant**, and less bug-filled than Visual Studio is enough for me to make the permanent switch. As I mentioned above, some small issues and missing features annoy me from time to time, but is nothing compared to freezes, lag and bugs from Visual Studio. 

I'd recommend that everyone gives Rider a go for at least a week (there's a 30 day free trial), because trust me, once you switch, you cannot go back!

NOTE: THIS REPO IS EXTREMELY IN-PROGRESS AT THE MOMENT.

THIS MESSAGE WILL BE REMOVED ONCE I HAVE A SIGNIFICANT NUMBER OF SETTINGS AVAILABLE.


# Index

* [Why didn't you just...?](https://github.com/PockyBum522/windows-command-line-settings#why-didnt-you-just)

* [What it does](https://github.com/PockyBum522/windows-command-line-settings#what-it-does)
* [Features and Roadmap](https://github.com/PockyBum522/windows-command-line-settings#features-and-roadmap)
* [Prerequisites](https://github.com/PockyBum522/windows-command-line-settings#prerequisites)
* [Usage](https://github.com/PockyBum522/windows-command-line-settings#usage)
* [Objectives](https://github.com/PockyBum522/windows-command-line-settings#objectives)

* [Detailed Breakdown of What's Going On in the Main Application](https://github.com/PockyBum522/windows-command-line-settings#detailed-breakdown-of-whats-going-on-in-the-main-application)
* [Helping With Development](https://github.com/PockyBum522/windows-command-line-settings#helping-with-development)

# Windows Command Line Settings


### What it does

Takes the pain out of new Windows 10/11 installations, by letting you install new software and configure settings, all with simple batch file lines that are easily changed.

NOTE: NOT ALL OF THESE ARE IMPLEMENTED YET
You should be able to:
1. Install Windows 10/11 on a computer
2. Connect to the internet
3. Run a batch file that you've set up to apply the settings/installs you want
4. Walk away

In about an hour, depending on your internet speed and system speed, you should reurn to find that:

NOTE: NOT ALL OF THESE ARE IMPLEMENTED YET
* Your new Windows install has all settings set how you like them
* Your Windows install is completely up to date (If you selected the "Update Windows" option)
* Any applications you selected for install are now ready
* Any necessary reboots are handled automatically, with the program resuming on next boot.
* Your computer has been renamed to your chosen hostname

Through the optional use of AutoLogon64 from Microsoft SysInternals, which the bootstrapper script will set up for you, you can make the whole process happen completely unattended. If you have security concerns, you can simply type in the user's password each time the script needs to reboot and resume. Temporarily disabling UAC is also completely optional, though you will need to do both (Configure autologon, and disable UAC, both of which are handled by the example batch file) at least temporarily to make the process unattended.


# Features and Roadmap

* See also: [Objectives](https://github.com/PockyBum522/windows-command-line-settings#objectives)

* 100% tested and working settings for all Windows settings checkboxes - In progress
* Basic application installation - Complete with chocolatey!
* Windows Update with reboots - In progress 

We are aiming to add more settings as the project progresses. We are still in the early stages of setting this up and making something that should be useful to everyone. Your help and pull requests are gratefully welcome!

# Prerequisites

* A Windows 10/11 installation that is not configured with your preferred settings or applications.

* A user account on said machine with administrator priveleges

* Internet on said machine


# Usage

For the end user, there are a few things you should know:

First off, if you just want to try it, download the latest release and then just double click "RUN ME FIRST (BOOTSTRAPPER).bat" BE AWARE THIS WILL INSTALL A FEW THINGS AND CHANGE ONE SETTING ON YOUR PC AS AN EXAMPLE

This will set up a few things necessary for unattended install, like prompting you to disable UAC prompts (You can re-enable them after the application is finished configuring your computer) and setting up Automatic Logon to your user with Microsoft's AutoLogon. (You can disable it after the install by running sysinternals autologon again. Both of these steps are optional, but required for a fully unattended process)

If you have chosen to install standard .exe or .msi installers, save them for the last part of your batch file, so that everything else can happen unattended.


# Argument Notes

All arguments can have leading or interspersed '-' or '.' characters, they are for formatting only and will be stripped before checking the command.

All arguments are also completely case-insensitive.

All commands can be run in a batch file by using the following format:

:: Friendly short variable for the exe path
set settingsExe="%~dp0WindowsCommandLineSettings.exe" 

:: Then invoke the exe and pass the argument to set the taskbar search bar to be completely hidden
"%settingsExe%" -Taskbar-Searchbar-SetHidden

:: Note that this would perform the same thing as the line above:
"%settingsExe%" -Taskbar-Search-bar-Set-Hidden

:: And so would this:
"%settingsExe%" -taskbarsearchbarsethidden


# List of Arguments

### Taskbar:

| Argument                         | Run as          | Functionality                                                     |
| -------------------------------- | --------------- | ----------------------------------------------------------------- |
| -TaskbarSearchBarCollapseToIcon  | ADMIN or USER   | Collapses the taskbar search bar so that it just shows an icon    |
| -TaskbarSearchBarSetHidden       | ADMIN or USER   | Hide the taskbar search bar completely                            |


# Objectives 

### Short-Term

* Integrating all settings from old GUI version of program

* Application settings would be nice to be able to be set, not just Windows settings - This will be more difficult to set up since I can't just index .reg files for some of those

* Better logging and failure recovery


### Long-Term

* Windows 11 Support (Has not yet been tested at all)


# Detailed Breakdown of What's Going On in the Batch File Bootstrapper

When you double click on "RUN ME FIRST (BOOTSTRAPPER).bat" a few things happen:

* The batch file creates a .lockfile in C:\Users\Public\Documents\ so that the second half of the script (the second half runs as the user) waits until after the first half (first half runs as admin because Chocolatey needs it) has finished to allow the second half (non-admin) part to proceed.

* The batch file checks if you have an internet connection, exits if you don't.

* The batch file checks if you ran it as admin, exits if you DID. 

(This is because while it's easy to elevate the process later, it's suprisingly hard to figure out what user originally ran the batch file if it's started as administrator by them when they run it.)

* The batch file then notifies the user they should disable UAC prompts and set up AutoLogin. 

This is for unattended capability, both of these things can be re-enabled/disabled once the application finishes running and the computer reboots for the last time. Both of these are optional, but required for the whole process being unattended.

* The batch file then installs Chocolatey, which we'll be using to install things

* The batch file installs the latest .NET SDK so we can build this application

* The batch file then installs powershell core and Notepad++ just to give some basic utilities to make troubleshooting easier.

* The admin half (first half) of the batch file now deletes the .lockfile and closes. 

Once the second half sees that the lockfile is gone, it knows everything it needs has been installed and it can proceed. 

* The first half does some basic nuget cleaning, restores the packages in the project, and then builds and runs the application.

# Detailed Breakdown of What's Going On in the Main Application

* When you make your selections and hit "Start Execution" the application immediately saves all of the slections to a JSON file stored in C:\Users\Public\Documents\ so that it can act on them later.

* It also creates a batch file in the Public Startup folder to re-run the application on subsequent reboots. 

This is deleted once the application has run through its complete process.

* If you selected to update windows, a CLI windows update handler will be installed, and windows will be updated as far as it will allow without a reboot. 

* The application then updates the JSON file in C:\Users\Public\Documents\ and saves what stage of the installation process/that it has gotten through the first stage

* When the computer is done rebooting to apply the windows updates, the batch file in the public startup folder launches, and re-runs the application.

* Upon re-launch, the application checks the settings it saved at the beginning, and what stage of the process it's on. 

It does any necessary work, and then if necessary, updates the stage and reboots the computer. The first few reboots are just to install windows updates, reboot, then see if there are any more updates and install them. This only happens if you checked the "Update Windows" checkbox.

* Once it's done updating windows, it will start installing selected applications. It handles all the selected 
ChocolateyInstallers, ArchiveInstallers, and PortableApplicationInstallers first, since those don't require user interaction. Once those are finished, it runs all ExecutableInstallers.

It will then load a warning dialog telling you to proceed through any user-interactive ExecutableInstallers that may be on the screen, then once you are finished, press yes to reboot the computer.

This is the last reboot, and at this point the process is finished. The application will clean up the files it was using to save the current stage of the process it was in, which are: The JSON file with stage info and the user options that were selected at the beginning, and the bat file in Public Startup.


# Helping With Development

So you read through all that? I'm impressed. Maybe a little frightened. I'm hesitant to put a lot of effort into this section at such an early stage of development, as things will likely change quickly.

However! Know that help and pull requests are GREATLY appreciated and I will review them in a timely manner.

For now, there's several things that would be helpful until I get a better idea of the structure of the code:

* Windows Settings

If you want a setting added and can get a .reg file or C# code to do what you want, send it over!

* Crash/Bug Reports

Log files are located in C:\Users\Public\Documents\Logs\

Please add an issue in github with the log and expected behavior. Logs shouldn't contain sensitive data (Possibly your username.) and if they do, let me know, so I can tell it to stop logging it.

* Code Review

I am self taught, and in addition to that, I don't know what I don't know.

If you see a better way to do something, or structure things, or anything, please talk to me about it. File an issue and use the "question" tag. I would love to have a conversation with you!

* Security Review

See above. I don't know what I don't know. 

The good news is this application isn't running on the open internet, it's running on your local machine. It also does not phone home or connect to anywhere. The bad news is it needs admin priveleges for a lot of the process. Protect your configuration files. Nasty stuff could be replicated on any machines that you use this on should something get compromised. However, the same holds true if you were just carrying around a thumbdrive with a bunch of .exe's to install and one of those became compromised. Also, fair warning, some of the preconfigured chocolatey installs might be community packages. Update those to be only official packages if you want better security checks on that side.


# Why didn't you just...?

Use group policy?

Use ninite? (I love ninite. Great product. I used the paid version for a long while.)

Use X, Y, or Z?

Mostly because I wanted to make something that's useful and usable by everyone! I wanted it to be simple to configure, powerful enough to be useful, and help real people save time. Since this application is meant to be run from easily-editable batch script files, changes are quick and easy.

Up to this point, I have done lots of research and have never been able to find something that will let you do Windows settings, application installs, and custom configuration of those things all in one place. This aims to fix that.

# Workout Tracker (Native App)

**Author:** This project was developed entirely by a single person.

Simple native workout tracking app built to log training sessions without subscriptions or unnecessary features.

The app allows users to create custom workouts, start and complete them with sets, repetitions, and timers, and store each finished workout as a detailed session in local history. All data is stored locally on the device using SQLite.

This project was developed as the **final project of a 6-month IT training program**. 
From which development process of the App itself took **7 weeks**.

## Technology
- C#
- .NET MAUI
- SQLite

## Architecture
- MVVM (Model–View–ViewModel)

## Core Features
- Create custom workouts
- Run workouts with sets, repetitions, and timers
- Store completed sessions in history
- Local database (no backend required)

## Exercise Data Source

The exercise data used in this project is based on the dataset from  
https://github.com/wrkout/exercises.json

For this project I used a modified version of that dataset provided here:  
https://github.com/yuhonas/free-exercise-db

The edited dataset was used as the source for exercises in the application.





---





## My Development Journey

At the beginning, the only thing I really knew was that I wanted to work with **C#** and the **.NET** ecosystem.

The project turned out to be difficult for a few reasons.

First, it was the first time I had to build something **completely from scratch**. There was no existing project or structure I could rely on, so I had to figure out everything step by step while building it.

Second, I had to learn a **new framework** and the architecture that comes with it — **MVVM**. Understanding how Models, ViewModels, and Views work together took some time before things started to make sense.

The **7-week timeframe** also made things harder.  
The first three weeks were mostly spent researching what technologies exist and what is commonly used.

At first I considered using **Entity Framework**, **ASP.NET**, and **.NET MAUI**. But after looking into them I realized there simply wasn’t enough time to learn all of them properly. Because of that, I decided to focus only on **.NET MAUI**, since it is the main framework used to build the application.

Even then it took me around **a week just to start understanding MVVM and how .NET MAUI structures an application**. At that point I was still missing a lot of knowledge, and I kept learning new things while actually building the project.

Another challenge was that this is a **native application**. Learning how to structure and build something like that properly was also a new experience for me. There is definitely still room for improvement, but it was a very useful learning process.

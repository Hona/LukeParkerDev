+++ 
date: "2021-02-01"
title: "Building a Beautiful OKR with AntBlazor - Part 0"
slug: "building-a-beautiful-okr-with-antblazor/0"
tags: ["Blazor", "AntDesign", "AntBlazor", "C#", "OKR"]
categories: ["Development"]
series: ["OKR"]
authors: ["Luke Parker"]
+++

## Introduction

### Why?

There are many boilerplate Blazor examples everywhere online, showing the basics, and some more intricate parts. However, I am yet to find a full blog series showing end-to-end, how to make a beautiful UI and clean coded backend 'real world' website with Blazor

### The Idea

For this series I decided that a reasonably simple but more complex than the typical TODO app, is an OKR website. If you aren't familiar with what an OKR tracker is, it stands for `objectives and key results`. What this means is there are larger objectives, that can be tracked by specific key results.

An example might be:

O: Become the top retailer in your state

KR:
 - Increase sales by 45%
 - Decrease monthly costs by $2000

From a developer perspective this is a simple enough app, you have **one** objective to **many** key results. This means the focus of this series will be to dive into UI and hooking it up with Blazor.

### Series Roadmap

1. Scaffold the project boilerplate using DDD (domain driven design), and full DevOps (GitHub CI, Docker + docker-compose, NGINX domain->docker setup)
2. Plan the backend, and setup the core models and repository interfaces
3. Build mock repositories with test data, to be able to build the UI properly, once. Build the main UI sections
4. Build the backend repositories and database setup using Marten DB as a document store
5. Build an analytics dashboard using data from all OKRs

#### Potential Extensions

* Add authentication, authorization, users/groups
* Add an API to expose data for 3rd party use

## The Tech Stack

Domain Registrar: **Cloudflare**

DDoS Protection, Nameservers, DNS: **Cloudflare**

Server Hosting: **Vultr** VPS

Reverse Proxy (differentiate between domains): **NGINX**

Containers: **Docker** and **docker-compose**

Website: **ASP.NET Core Blazor + C#**

Database: **Postgres**

## UI

There are many UI frameworks for Blazor, but many are hard to use, require knowledge from React and more. The only UI framework I've found for Blazor that makes sense, and is easy to use - is [AntBlazor](https://antblazor.com/). AntBlazor is a Blazor component library for [Ant Design](https://ant.design/).

This library supports many components, and its component overview can be found on the linked site. I've personally used AntBlazor for many of my personal projects, and for paid projects - in production. It is still a somewhat young library compared to React or Angular UI libraries, but Blazor is the new tech on the block.

In this blog series I am going to show you how easy it is to build a beautiful frontend; this is especially useful if you are primarily a backend developer and don't have/want to put in the time for the frontend.

---

In the next post, I will setup the project boilerplate and full DevOps.

[Part 1 →](https://lukeparker.dev/posts/building-a-beautiful-okr-with-antblazor/1)
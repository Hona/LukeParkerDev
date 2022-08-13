+++ 
date: "2021-02-04"
title: "Building a Beautiful OKR with AntBlazor - Part 2"
slug: "building-a-beautiful-okr-with-antblazor/2"
tags: ["Blazor", "AntDesign", "AntBlazor", "C#"]
categories: ["Development"]
series: ["OKR"]
hook: "Building out the backend with core models and repository interfaces. Then, we mock the data for UI building!"
+++

## Planning the Backend

There is one objective with many key results. I added some properties that I think will be useful. These may change as we figure out the UI/UX a bit more.

Some feature ideas:
* Objectives should be toggleable if they are active or not
* Objectives should have a start and end date
* Objectives should have a brief title, and a longer description
* Key Results should have a brief title, and a longer description
* Key Results should be toggleable if they are active or not
* Key Results should have a completion percent 0%-100%
* Key Results should have a priority (higher the number the more important)

```csharp
public class Objective
{
    public Guid Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public DateTime DueDate { get; set; }
    public DateTime StartDate { get; set; }
    public bool Active { get; set; }
}
```

```csharp
public class KeyResult
{
    public Guid Id { get; set; }
    public Guid ObjectiveId { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public bool Active { get; set; }
    public decimal Completion { get; set; }
    public int Priority { get; set; }
}
```

That's it for the core models! Lets add it to the project...

## Coding it

In the `OKR.Core` project, create a folder called `Models` - you guessed it, for our domain models. Now add the two models from above in files: `Objective.cs` and `KeyResult.cs`

### Repositories

Now we have the core models, we need to build an interface that abstracts data access. Lets build two repositories for the Objectives and Key Results.

Create another folder in `OKR.Core` called `Repositories`.

Data access code, especially in repositories can get pretty repetitive, so lets abstract it to a generic repository:

IGenericRepository.cs
```csharp
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OKR.Core.Repositories
{
    public interface IGenericRepository<T>
    {
        public Task AddAsync(T model);
        public Task UpdateAsync(T model);
        public Task DeleteAsync(T model);

        public Task<IReadOnlyList<T>> GetAllAsync();
        public Task<IReadOnlyList<T>> GetPagedAsync(int page = 0);
    }
}
```

This code provides the simple CRUD operations on any model `T`

Now lets build the specific repositories for OKRs:

IObjectiveRepository.cs
```csharp
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using OKR.Core.Models;

namespace OKR.Core.Repositories
{
    public interface IObjectiveRepository : IGenericRepository<Objective>
    {
        Task<IReadOnlyList<Objective>> GetAllActiveAsync(bool active = true);
        Task<IReadOnlyList<Objective>> GetAllWithinDueDateAsync();
        Task<Objective> GetByIdAsync(Guid id);
    }
}
```

Note the added functions that are specific to `Objective` properties

IKeyResultRepository.cs
```csharp
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using OKR.Core.Models;

namespace OKR.Core.Repositories
{
    public interface IKeyResultRepository : IGenericRepository<KeyResult>
    {
        Task<IReadOnlyList<KeyResult>> GetAllByObjectiveAsync(Guid objectiveId);
    }
}
```

Pretty simple here, I don't think we need to provide any filtering on the data access level here, since there won't be any ridiculous number of key results inside an objective

## Mocking the Repositories

Before we get started with the frontend we have to mock the repositories to get some 'real' data.

Lets create a new class library project to hold the mock repositories called `OKR.Core.Mock`. Add a project reference to `OKR.Core`. Create a folder called `Repositories` and create the files: `MockObjectiveRepository.cs` and `MockKeyResultRepository.cs` - implementing the underlying interface. You should be able to autogenerate the functions like so:

```csharp
public class MockObjectiveRepository : IObjectiveRepository
    {
        public async Task AddAsync(Objective model)
        {
            throw new NotImplementedException();
        }

        public async Task UpdateAsync(Objective model)
        {
            throw new NotImplementedException();
        }
        
        ...
```

At this point, lets just make a couple functions return mock data, the rest can throw exceptions:

MockKeyResultRepository.cs
```csharp
public async Task<IReadOnlyList<KeyResult>> GetAllByObjectiveAsync(Guid objectiveId) => new[]
{
    new KeyResult
    {
        Id = Guid.NewGuid(),
        Active = true,
        Completion = 0,
        Description = "Do something...",
        Title = "Something",
        Priority = 1,
        ObjectiveId = objectiveId
    },
    new KeyResult
    {
        Id = Guid.NewGuid(),
        Active = true,
        Completion = (decimal) 0.35,
        Description = "Do this...",
        Title = "This",
        Priority = 4,
        ObjectiveId = objectiveId
    }
    ,
    new KeyResult
    {
        Id = Guid.NewGuid(),
        Active = true,
        Completion = (decimal) 0.80,
        Description = "Do that...",
        Title = "This",
        Priority = 10,
        ObjectiveId = objectiveId
    }
};
```

MockObjectiveRepository.cs
```csharp
public async Task<IReadOnlyList<Objective>> GetAllAsync() => new[]
{
    new Objective
    {
        Active = true,
        Title = "Q1 Growth",
        Description = "Grow in Q1 by doing some productive things.",
        Id = Guid.NewGuid(),
        StartDate = DateTime.Now.Subtract(TimeSpan.FromDays(4)),
        DueDate = DateTime.Now.AddDays(5)
    },
    new Objective
    {
        Active = true,
        Title = "Lorem Ipsum",
        Description = "A sentence describing what this is",
        Id = Guid.NewGuid(),
        StartDate = DateTime.Now.Subtract(TimeSpan.FromDays(4)),
        DueDate = DateTime.Now.AddDays(5)
    },
    new Objective
    {
        Active = true,
        Title = "Q2 Growth",
        Description = "Grow in Q2 by doing some productive things.",
        Id = Guid.NewGuid(),
        StartDate = DateTime.Now.Subtract(TimeSpan.FromDays(20)),
        DueDate = DateTime.Now.AddDays(5)
    },
    new Objective
    {
        Active = true,
        Title = "Q3 Growth",
        Description = "Grow in Q3 by doing some productive things.",
        Id = Guid.NewGuid(),
        StartDate = DateTime.Now.Subtract(TimeSpan.FromDays(50)),
        DueDate = DateTime.Now.AddDays(100)
    },
    new Objective
    {
        Active = false,
        Title = "Q4 Growth",
        Description = "Grow in Q4 by doing some productive things.",
        Id = Guid.NewGuid(),
        StartDate = DateTime.Now.AddDays(100),
        DueDate = DateTime.Now.AddDays(200)
    },
};
```

Now we have some mock repositories return *some* data.

---

In the next post, I begin the UI with AntBlazor, using the mock data.

[← Part 1](https://lukeparker.dev/posts/building-a-beautiful-okr-with-antblazor/1) | [Part 3 →](https://lukeparker.dev/posts/building-a-beautiful-okr-with-antblazor/3)
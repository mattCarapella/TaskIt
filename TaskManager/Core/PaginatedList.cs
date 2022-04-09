﻿using Microsoft.EntityFrameworkCore;

namespace TaskManager.Core;

public class PaginatedList<T> : List<T>
{
    public int PageIndex { get; set; }
    public int TotalPages { get; set; }

    public PaginatedList(List<T> items, int count, int pageIndex, int pageSize)
    {
        PageIndex = pageIndex;
        TotalPages = (int)Math.Ceiling(count / (double)pageSize);
        this.AddRange(items);
    }

    public bool HasPreviousPage => PageIndex > 1;

    public bool HasNextPage => PageIndex < TotalPages;

    public static PaginatedList<T> Create(List<T> source, int pageIndex, int pageSize)
    {
        var count = source.Count();
        var items = source.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
        return new PaginatedList<T>(items, count, pageIndex, pageSize);
    }

}



//public class PaginatedListAsync2<T> : List<T>
//{
//    public int PageIndex { get; set; }
//    public int TotalPages { get; set; }

//    public PaginatedListAsync2(List<T> items, int count, int pageIndex, int pageSize)
//    {
//        PageIndex = pageIndex;
//        TotalPages = (int)Math.Ceiling(count / (double)pageSize);
//        this.AddRange(items);
//    }

//    public bool HasPreviousPage => PageIndex > 1;

//    public bool HasNextPage => PageIndex < TotalPages;

//    public static async Task<PaginatedListAsync2<T>> CreateAsync(IQueryable<T> source, int pageIndex, int pageSize)
//    {
//        var count = await source.CountAsync();
//        var items = await source.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync();
//        return new PaginatedListAsync2<T>(items, count, pageIndex, pageSize);
//    }

//}
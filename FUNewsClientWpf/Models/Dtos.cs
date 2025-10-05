using System;
using System.Collections.Generic;

namespace FUNewsClientWpf.Models
{
    public sealed class LoginRequest
    {
        public string? Email { get; set; }
        public string? Password { get; set; }
    }

    public sealed class LoginResult
    {
        public string? message { get; set; }
        public string? role { get; set; } // "Admin" or "Staff", etc.
        public short accountId { get; set; }
    }

    public sealed class MeResult
    {
        public string? accountId { get; set; }
        public string? name { get; set; }
        public string? email { get; set; }
        public string? role { get; set; }
    }

    public sealed class SystemAccount
    {
        public short AccountId { get; set; }
        public string? AccountName { get; set; }
        public string? AccountEmail { get; set; }
        public string? AccountPassword { get; set; }
        public int? AccountRole { get; set; }
    }

    public sealed class Category
    {
        public short CategoryId { get; set; }
        public string? CategoryName { get; set; }
        public string? CategoryDesciption { get; set; }
        public short? ParentCategoryId { get; set; }
        public bool? IsActive { get; set; }
    }

    public sealed class NewsArticle
    {
        public string? NewsArticleId { get; set; }
        public string? NewsTitle { get; set; }
        public string Headline { get; set; } = string.Empty;
        public string? NewsContent { get; set; }
        public string? NewsSource { get; set; }
        public short? CategoryId { get; set; }
        public bool? NewsStatus { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public short? CreatedById { get; set; }
        public short? UpdatedById { get; set; }
        public List<Tag> Tags { get; set; } = new();
    }

    public sealed class CreateNewsArticleRequest
    {
        public string? NewsTitle { get; set; }
        public string Headline { get; set; } = string.Empty;
        public string? NewsContent { get; set; }
        public string? NewsSource { get; set; }
        public short? CategoryId { get; set; }
        public bool? NewsStatus { get; set; }
    }

    public sealed class UpdateNewsArticleRequest
    {
        public string? NewsTitle { get; set; }
        public string? Headline { get; set; }
        public string? NewsContent { get; set; }
        public string? NewsSource { get; set; }
        public short? CategoryId { get; set; }
        public bool? NewsStatus { get; set; }
    }

    public sealed class UpdateTagsRequest
    {
        public List<int> TagIds { get; set; } = new();
    }

    public sealed class Tag
    {
        public int TagId { get; set; }
        public string? TagName { get; set; }
        public string? Note { get; set; }
    }
}
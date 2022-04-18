using System;
using System.Collections.Generic;
using System.Text;

namespace ReadingCompanion
{
    public enum SortBy
    {
        Title,
        Rating,
        Genre,
    }
    public enum Status
    {
        Reading,
        OnHold,
        Backlog,
        Completed,
        Dropped,
    }
    public class ItemModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public float ReadProgress { get; set; }
        public float ReadTotal { get; set; }
        public double Rating { get; set; }
        public string Genre { get; set; }
        public string SubGenre1 { get; set; }
        public string SubGenre2 { get; set; }
        public string SubGenre3 { get; set; }

        public Status Status { get; set; }
        public string ImageURL { get; set; }
    }
}

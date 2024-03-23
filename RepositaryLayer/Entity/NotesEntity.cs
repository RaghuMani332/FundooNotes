using BuisinessLayer.Entity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace RepositaryLayer.Entity
{
    public class NotesEntity
    {
        public int NoteId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string BgColor { get; set; }
        public string ImagePath { get; set; } 
        public DateTime? Remainder { get; set; }
        public bool IsArchive { get; set; }
        public bool IsPinned { get; set; }
        public bool IsTrash { get; set; }//(When create notes this should be false)
        public DateTime CreatedAt { get; set; }
        public DateTime ModifiedAt { get; set; }
        public List<int> CollabId { get; set; }
        public int UserId { get; set; }
        public String LableName {  get; set; }
        public override string ToString()
        {
            return $"NoteId: {NoteId}, Title: {Title}, Description: {Description}, BgColor: {BgColor}, ImagePath: {ImagePath}, Remainder: {Remainder}, IsArchive: {IsArchive}, IsPinned: {IsPinned}, IsTrash: {IsTrash}, CreatedAt: {CreatedAt}, ModifiedAt: {ModifiedAt}, CollabId: {CollabId}, UserId: {UserId}";
        }
    }
}

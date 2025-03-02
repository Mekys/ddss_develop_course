using Domain.Post.Events;
using FluentResults;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Post.ValueObjects
{
    public record Like
    {
        public Guid LikedById { get; set; }
        public DateTime CreatedAtUtc { get; set; }
        
        private Like(
            Guid likedById, 
            DateTime createdAtUtc)
        {
            LikedById = likedById;
            CreatedAtUtc = createdAtUtc;
        }

        public static Result<Like> Create(
            Guid likedById, 
            DateTime createdAtUtc)
        {
            if (likedById == default)
            {
                return Result.Fail(new LikedByIdNotSetError());
            }

            if (createdAtUtc > DateTime.UtcNow)
            {
                return Result.Fail(new FutureTimeError());
            }
          
            return new Like(likedById, createdAtUtc);
        }
    }
}

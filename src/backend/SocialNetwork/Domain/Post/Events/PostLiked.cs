using Domain.Post.ValueObjects;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Post.Events
{
    public class PostLiked : INotification
    {
        public Like Like { get; private set; } 
        public Guid PostId { get; private set; }

        public PostLiked(Like like, Guid postId)
        {
            Like = like;
            PostId = postId;
        }

    }
}

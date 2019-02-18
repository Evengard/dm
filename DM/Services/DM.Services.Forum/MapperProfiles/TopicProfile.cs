using System.Linq;
using AutoMapper;
using DM.Services.DataAccess.BusinessObjects.Common;
using DM.Services.DataAccess.BusinessObjects.Fora;
using DM.Services.Forum.Dto;

namespace DM.Services.Forum.MapperProfiles
{
    public class TopicProfile : Profile
    {
        public TopicProfile()
        {
            CreateMap<LastComment, Comment>();
            CreateMap<ForumTopic, TopicsListItem>()
                .ForMember(d => d.Id, s => s.MapFrom(t => t.ForumTopicId))
                .ForMember(d => d.LastActivityDate, s => s.MapFrom(t => t.LastComment == null
                    ? t.CreateDate
                    : t.LastComment.CreateDate))
                .ForMember(d => d.TotalCommentsCount, s => s.MapFrom(t => t.Comments.Count(c => !c.IsRemoved)));
        }
    }
}
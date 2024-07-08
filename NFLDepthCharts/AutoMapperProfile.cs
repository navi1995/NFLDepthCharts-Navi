namespace NFLDepthCharts.API
{
    using AutoMapper;
    using NFLDepthCharts.API.DTOs;
    using NFLDepthCharts.API.Models;

    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Player, PlayerDto>();
            CreateMap<Position, PositionDto>();
            CreateMap<DepthChartEntry, DepthChartEntryDto>()
                .ForMember(dest => dest.Depth, opt => opt.MapFrom(src => src.DepthLevel));
            CreateMap<AddPlayerToDepthChartDto, Player>()
                .ForMember(dest => dest.PlayerId, opt => opt.Ignore())
                .ForMember(dest => dest.Number, opt => opt.MapFrom(src => src.PlayerNumber))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.PlayerName))
                .ForMember(dest => dest.PositionPlayerDepthEntries, opt => opt.Ignore());

            CreateMap<IDictionary<string, IEnumerable<Player>>, FullDepthChartDto>()
                .ConvertUsing((src, dest, context) => new FullDepthChartDto
                {
                    Positions = src.ToDictionary(
                        kvp => kvp.Key,
                        kvp => context.Mapper.Map<List<PlayerDto>>(kvp.Value.ToList())
                    )
                });
        }
    }
}

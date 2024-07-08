using AutoMapper;
using NFLDepthCharts.API.Constants;
using NFLDepthCharts.API.DTOs;
using NFLDepthCharts.API.Exceptions;
using NFLDepthCharts.API.Models;
using NFLDepthCharts.API.Repositories;
using NFLDepthCharts.API.Validators;

namespace NFLDepthCharts.API.Services
{
    public class DepthChartService : IDepthChartService
    {
        private readonly IPlayerRepository _playerRepository;
        private readonly IPositionRepository _positionRepository;
        private readonly IDepthChartRepository _depthChartRepository;
        private readonly IMapper _mapper;
        private readonly IPlayerValidator _playerValidator;
        private readonly IPositionValidator _positionValidator;
        private readonly IDepthChartEntryValidator _depthChartEntryValidator;
        private readonly IAddPlayerToDepthChartDtoValidator _addPlayerToDepthChartDtoValidator;

        public DepthChartService(
            IPlayerRepository playerRepository,
            IPositionRepository positionRepository,
            IDepthChartRepository depthChartRepository,
            IMapper mapper,
            IPlayerValidator playerValidator,
            IPositionValidator positionValidator,
            IDepthChartEntryValidator depthChartEntryValidator,
            IAddPlayerToDepthChartDtoValidator addPlayerToDepthChartDtoValidator)
        {
            _playerRepository = playerRepository;
            _positionRepository = positionRepository;
            _depthChartRepository = depthChartRepository;
            _mapper = mapper;
            _playerValidator = playerValidator;
            _positionValidator = positionValidator;
            _depthChartEntryValidator = depthChartEntryValidator;
            _addPlayerToDepthChartDtoValidator = addPlayerToDepthChartDtoValidator;
        }

        public async Task<bool> AddPlayerToDepthChart(AddPlayerToDepthChartDto dto)
        {
            _addPlayerToDepthChartDtoValidator.Validate(dto);
            var positionEntity = await _positionRepository.GetByNameAsync(dto.Position);
            _positionValidator.Validate(positionEntity);
            var playerEntity = await _playerRepository.GetByNumberAsync(dto.PlayerNumber);

            if (playerEntity == null)
            {
                playerEntity = _mapper.Map<Player>(dto);
                _playerValidator.Validate(playerEntity);
                playerEntity = await _playerRepository.AddAsync(playerEntity);
            }

            var maxDepth = await _depthChartRepository.GetMaxDepthForPositionAsync(positionEntity.PositionId);
            var depth = dto.PositionDepth ?? maxDepth + 1;

            var newEntry = new DepthChartEntry
            {
                PlayerId = playerEntity.PlayerId,
                PositionId = positionEntity.PositionId,
                DepthLevel = depth
            };

            _depthChartEntryValidator.Validate(newEntry);

            var entries = await _depthChartRepository.GetEntriesByPositionAsync(positionEntity.PositionId);
            var entriesToUpdate = entries.Where(d => d.DepthLevel >= depth).ToList();

            foreach (var entry in entriesToUpdate)
            {
                entry.DepthLevel++; // Shift other existing entries up higher, new entry gets prio
            }

            await _depthChartRepository.UpdateEntriesDepthAsync(entriesToUpdate);
            await _depthChartRepository.AddEntryAsync(newEntry);

            return true;
        }

        public async Task<PlayerDto> RemovePlayerFromDepthChart(string position, int playerNumber)
        {
            var positionEntity = await _positionRepository.GetByNameAsync(position);
            _positionValidator.Validate(positionEntity);

            var playerEntity = await _playerRepository.GetByNumberAsync(playerNumber);
            _playerValidator.Validate(playerEntity);

            var removedEntry = await _depthChartRepository.RemoveEntryAsync(positionEntity.PositionId, playerEntity.PlayerId);

            if (removedEntry == null)
            {
                throw new ValidationException(ErrorMessages.PlayerNotInDepthChart);
            }

            var entries = await _depthChartRepository.GetEntriesByPositionAsync(positionEntity.PositionId);
            var entriesToUpdate = entries.Where(d => d.DepthLevel > removedEntry.DepthLevel).ToList();

            foreach (var entry in entriesToUpdate)
            {
                entry.DepthLevel--; // Shift any other remaining entries lower to make up the gap
            }

            await _depthChartRepository.UpdateEntriesDepthAsync(entriesToUpdate);

            return _mapper.Map<PlayerDto>(removedEntry.Player);
        }

        public async Task<IEnumerable<PlayerDto>> GetBackups(string position, int playerNumber)
        {
            var positionEntity = await _positionRepository.GetByNameAsync(position);
            _positionValidator.Validate(positionEntity);

            var playerEntity = await _playerRepository.GetByNumberAsync(playerNumber);
            _playerValidator.Validate(playerEntity);

            var backupEntries = await _depthChartRepository.GetBackupsAsync(positionEntity.PositionId, playerEntity.PlayerId);

            return _mapper.Map<IEnumerable<PlayerDto>>(backupEntries.Select(e => e.Player));
        }

        public async Task<FullDepthChartDto> GetFullDepthChart()
        {
            var depthChart = await _depthChartRepository.GetFullDepthChartAsync();

            return _mapper.Map<FullDepthChartDto>(depthChart);
        }
    }
}

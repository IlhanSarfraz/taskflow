using TaskFlow.Domain.Common;

namespace TaskFlow.Domain.Entities
{
    public sealed class TaskAttachment : BaseEntity
    {
        public Guid TaskId { get; set; }

        public string FileName { get; set; } = default!;

        public string ContentType { get; set; } = default!;

        public long FileSize { get; set; }

        public string CloudinaryPublicId { get; set; } = default!;

        public string Url { get; set; } = default!;

        public Guid UploadedBy { get; set; }

        public TaskItem Task { get; set; } = default!;
    }
}

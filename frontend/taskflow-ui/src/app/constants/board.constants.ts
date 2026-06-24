export const DEFAULT_COLUMN_NAMES = ['To Do', 'In Progress', 'Done'] as const;
export type DefaultColumnName = typeof DEFAULT_COLUMN_NAMES[number];
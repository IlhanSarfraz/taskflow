import { Pipe, PipeTransform } from "@angular/core";

@Pipe({
    name: 'relativeTime',
    standalone: true
})
export class RelativeTimePipe implements PipeTransform{
    transform(dateStr: string): string {
        if(!dateStr) return '';
        
        const utc = dateStr.endsWith('Z') ? dateStr : dateStr + 'Z';
            const date = new Date(utc);
            const now = new Date();
            const diffMs = now.getTime() - date.getTime();
            const diffMins = Math.floor(diffMs / 60000);
            const diffHours = Math.floor(diffMins / 60);
            const diffDays = Math.floor(diffHours / 24);

            if (diffMins < 1)   return 'Just now';
            if (diffMins < 60)  return `${diffMins} minute${diffMins > 1 ? 's' : ''} ago`;
            if (diffHours < 24) return `${diffHours} hour${diffHours > 1 ? 's' : ''} ago`;
            if (diffDays < 7)   return `${diffDays} day${diffDays > 1 ? 's' : ''} ago`;

            return date.toLocaleDateString('en-US', {
            month: 'short', day: 'numeric', year: 'numeric'
        });
    }
}
import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { Observable } from "rxjs";
import { environment } from "../../../environments/environment";

@Injectable({
    providedIn: 'root'
})
export class ApiService{
    constructor(private http: HttpClient) {}

    get<T>(url:string){
        return this.http.get<T>(`${environment.apiUrl}/${url}`)
    }

    post<T>(url:string, body: any){
        return this.http.post<T>(`${environment.apiUrl}/${url}`, body)
    }

    put<T>(url:string, body: any){
        return this.http.put<T>(`${environment.apiUrl}/${url}`, body)
    }

    patch<T>(url: string, body: any = {}) {
        return this.http.patch<T>(`${environment.apiUrl}/${url}`, body);
    }

    delete<T>(url:string){
        return this.http.delete<T>(`${environment.apiUrl}/${url}`)
    }

    getBlob(url: string): Observable<Blob> {
        return this.http.get(`${environment.apiUrl}/${url}`, { responseType: 'blob' });
    }
}
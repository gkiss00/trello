import {Injectable, Inject} from '@angular/core';
import {HttpClient} from '@angular/common/http';

import { map } from 'rxjs/operators';
import { AuthenticationService } from './authentication.service';
import { Section } from '../models/section';
import { Card } from '../models/card';

@Injectable({ providedIn: 'root' })
export class SectionService {

    constructor(private http: HttpClient, @Inject('BASE_URL') private baseUrl: string, private authenticationService: AuthenticationService){}

    addCard(title: string, sectionId: number, position: number, authorId: number) {
        return this.http.post<Card>(`${this.baseUrl}api/cards`, { title, sectionId, position, authorId })
            .pipe(map(card => {
                card = new Card(card);   
            return card;
        }));
    }

    deleteSection(sectionId: number){
        return this.http.delete(`${this.baseUrl}api/sections/${sectionId}`);
    }

    updateSectionTitle(sectionId: number, title: string){
        return this.http.put<Section>(`${this.baseUrl}api/sections/${sectionId}`, { title });
    }

}
import {Injectable, Inject} from '@angular/core';
import {HttpClient} from '@angular/common/http';

import { map } from 'rxjs/operators';
import { Card } from '../models/card';

@Injectable({ providedIn: 'root' })
export class CardService {

    constructor(private http: HttpClient, @Inject('BASE_URL') private baseUrl: string){}

    updateCardPosition(cardId: number, position: number) {
        return this.http.put<Card>(`${this.baseUrl}api/cards/${cardId}`, { position });
    }

    updateCardSection(sectionId: number, cardId: number, position: number) {
        return this.http.put<Card>(`${this.baseUrl}api/cards/${cardId}`, { sectionId, position });
    }

    getCurrentCard(cardId: number){
        return this.http.get<Card>(`${this.baseUrl}api/cards/${cardId}`).pipe(map(card => new Card(card)));
    }

    updateCardContent(cardId: number, content: string){
        return this.http.put<Card>(`${this.baseUrl}api/cards/${cardId}`, { content }).pipe(map(card => new Card(card)));
    }

    updateCardTitle(cardId: number, title: string){
        return this.http.put<Card>(`${this.baseUrl}api/cards/${cardId}`, { title }).pipe(map(card => new Card(card)));
    }

    addMember(cardId: number, userId: number){
        let action: string = "Add";
        return this.http.put<Card>(`${this.baseUrl}api/cards/${cardId}`, { userId, action });
    }

    removeMember(cardId: number, userId: number){
        let action: string = "Remove"
        return this.http.put<Card>(`${this.baseUrl}api/cards/${cardId}`, { userId, action });
    }

    deleteCard(cardId: number){
        return this.http.delete(`${this.baseUrl}api/cards/${cardId}`);
    }

}



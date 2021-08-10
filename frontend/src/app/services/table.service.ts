import {Injectable, Inject} from '@angular/core';
import {HttpClient} from '@angular/common/http';

import { map } from 'rxjs/operators';
import { Section } from '../models/section';
import { Table } from '../models/table';
import { User } from '../models/user';
import { __core_private_testing_placeholder__ } from '@angular/core/testing';
import { Team } from '../models/team';

@Injectable({ providedIn: 'root' })
export class TableService {

    constructor(private http: HttpClient, @Inject('BASE_URL') private baseUrl: string){}

    addSection(title: string, tableId: number) {
        return this.http.post<Section>(`${this.baseUrl}api/sections`, { title, tableId })
            .pipe(map(section => {
                section = new Section(section);   
            return section;
        }));
    }

    addMemberToTable(tableId: number, userId: number){
        let action: string = "Add";
        return this.http.put<Table>(`${this.baseUrl}api/tables/${tableId}`, { userId, action });   
    }

    removeMember(tableId: number, userId: number){
        let action: string = "Remove";
        return this.http.put<Table>(`${this.baseUrl}api/tables/${tableId}`, { userId, action });
    }

    deleteTable(tableId: number){
        return this.http.delete(`${this.baseUrl}api/tables/${tableId}`);
    }

    create(title : string, view : number, teamId : number, organizationId : number, usersInString : string[])
    {
        if (view == 0)
        {
            return this.http.post<Table>(`${this.baseUrl}api/tables/`, { title, usersInString })
                .pipe(map(table => {
                    table = new Table(table);
                return table;
                }));

        }
        else if (view == 1)
        {
            return this.http.post<Table>(`${this.baseUrl}api/tables/`, { title, teamId })
                .pipe(map(table => {
                    table = new Table(table);
                return table;
                }));

        }
        else
        {
            return this.http.post<Table>(`${this.baseUrl}api/tables/`, { title, organizationId })
                .pipe(map(table => {
                    table = new Table(table);
                return table;
                }));
        }
    }

}
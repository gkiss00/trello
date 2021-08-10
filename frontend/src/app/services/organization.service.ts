import {Injectable, Inject} from '@angular/core';
import {HttpClient} from '@angular/common/http';

import { User } from '../models/user';
import { Team } from '../models/team';
import { map } from 'rxjs/operators';
import { AuthenticationService } from './authentication.service';
import { Organization } from '../models/organization';

@Injectable({ providedIn: 'root' })
export class OrganizationService {
    constructor(private http: HttpClient, @Inject('BASE_URL') private baseUrl: string, private authenticationService: AuthenticationService){}

    getCurrentMembers(organizationId: number) {
        return (this.http.get<User[]>(`${this.baseUrl}api/organizations/${organizationId}/members`)
        .pipe(map(res => res.map(m => new User(m)))));
    }
    
    getOrganization(orgId: number) {
        return (this.http.get<Organization>(`${this.baseUrl}api/organizations/${orgId}`)
        .pipe(map(res => res)));
    }

    getCurrentTeams(orgId: number) {
        return (this.http.get<Team[]>(`${this.baseUrl}api/organizations/${orgId}/teams`)
        .pipe(map(res => res.map(m => new Team(m)))));
    }

    create(name : string, userList : string[], teams : Team[]) 
    {
        return (this.http.post<Organization>(`${this.baseUrl}api/organizations/`, {name, userList, teams})
            .pipe(map(orga => {
                orga = new Organization(orga);
            return orga;
            })));
    }

    removeUser(pseudo : string, id : number)
    {
        return (this.http.get<Organization>(`${this.baseUrl}api/organizations/rem_user/${pseudo}/${id}`)
            .pipe(map(orga => orga)));
    }

    delete(id : number)
    {
        return (this.http.delete<Organization>(`${this.baseUrl}api/organizations/${id}`)
            .pipe(map(orga => orga)));
    }

    removeTeam(orgId : number, teamId : number)
    {
        return (this.http.get<Organization>(`${this.baseUrl}api/organizations/${orgId}/${teamId}`)
            .pipe(map(orga => orga)));
    }

    addUserToOrga(userId : number, orgId : number)
    {
        return (this.http.get<User>(`${this.baseUrl}api/organizations/add/${orgId}/${userId}`)
            .pipe(map(user => user)));
    }

    getTeamsAddable(orgId : number)
    {       
        return (this.http.get<Team[]>(`${this.baseUrl}api/organizations/${orgId}/teams_addable`)
            .pipe(map(res => res.map(m => new Team(m)))));
    }

    addTeamToOrga(orgId : number, teamId : number){
        return (this.http.get<Team>(`${this.baseUrl}api/organizations/${orgId}/add_team/${teamId}`)
            .pipe(map(res => res)));
    }
    
}
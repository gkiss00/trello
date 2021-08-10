import { Component } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { Team } from 'src/app/models/team';
import { Organization } from 'src/app/models/organization';
import { AuthenticationService } from 'src/app/services/authentication.service';
import { UserService } from 'src/app/services/user.service';
import { User, Role} from 'src/app/models/user';

@Component({
    selector: 'left-sidebar',
    templateUrl: './left-sidebar.component.html',
    styleUrls: ['./left-sidebar.component.css']
})
export class LeftSidebarComponent {

    teams : Team[] = [];

    organizations : Organization[] = [];

    selectedTeam: number;

    selectedOrganization: number;

    toggleTeam(id: number): void {
        if(this.selectedTeam == id){
            this.selectedTeam = -1;
        } else{
            this.selectedTeam = id;
        }
    }

    toggleOrganization(id: number): void {
        if(this.selectedOrganization == id){
            this.selectedOrganization = -2;
        } else{
            this.selectedOrganization = id;
        }
    }

    constructor(
        private userService: UserService,
        private authenticationService: AuthenticationService,
        private activeRoute: ActivatedRoute,
        private router: Router
    ) {
        this.selectedTeam = this.activeRoute.snapshot.paramMap.get('id') ? +this.activeRoute.snapshot.paramMap.get('id') : -1;
        if(this.activeRoute.snapshot.paramMap.get('board') == "board"){
            this.selectedOrganization = this.activeRoute.snapshot.paramMap.get('id') ? +this.activeRoute.snapshot.paramMap.get('id') : -2;
            this.selectedTeam = -1;
        }
        userService.getCurrentUserTeams().subscribe(teams => {
            this.teams = teams;
        });
        userService.getCurrentUserOrganizations().subscribe(organizations => {
            this.organizations = organizations;
        });
    }

    get currentUser() {
        return this.authenticationService.currentUser;
    }

    isHomeRoute(teamId: number) {
        this.isOrgRoute(-2);
        return this.router.url === '/' + teamId + '/tables';
    }

    isOrgRoute(orgId: number) {
        return this.router.url === '/organization/' + orgId + '/tables';
    }

    isAdmin() : boolean{
        return (this.currentUser.role == Role.Admin)
    }

}

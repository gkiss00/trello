import { Input } from '@angular/core';
import { OnInit } from '@angular/core';
import { Component} from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { Organization } from 'src/app/models/organization';
import { Table } from 'src/app/models/table';
import { Team } from 'src/app/models/team';
import { AuthenticationService } from 'src/app/services/authentication.service';
import { OrganizationService } from 'src/app/services/organization.service';
import { UserService } from 'src/app/services/user.service';

@Component({
    selector: 'orga-table',
    templateUrl: 'single-orga-table.component.html',
    styleUrls: ['single-orga-table.component.css']
})
export class SingleOrgaTableComponent implements OnInit {

    tables: Table[] = [];
    teams : Team[] = [];
    
    constructor(
        private router: Router,
        private activeRoute: ActivatedRoute,
        private userService: UserService,
        private authenticationService: AuthenticationService,
        private orgService: OrganizationService
    ) { 
    }

    ngOnInit(): void {
        this.activeRoute.params.subscribe(routeParams => {
            let orgId = +this.activeRoute.snapshot.paramMap.get('id');
            if(orgId > 0){
                this.userService.getCurrentUserOrga(orgId).subscribe(t => {
                        this.teams = t;
                    },
                    error => {
                        this.router.navigate(['/unknown']);
                });
            }
        });
    }

    titleUrlConverter(title: string): string{
        return title.replace(/\s/g, "-");
    }
    
    get currentUser() {
        return this.authenticationService.currentUser;
    }

}
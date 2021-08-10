import { Component} from '@angular/core';
import { Table } from 'src/app/models/table';
import { Team } from 'src/app/models/team';
import { AuthenticationService } from 'src/app/services/authentication.service';
import { UserService } from 'src/app/services/user.service';

@Component({
    templateUrl: 'table.component.html',
    styleUrls: ['table.component.css']
})
export class TableComponent {
    tables: Table[] = [];
    teams : Team[] = [];

    constructor(
        private userService: UserService,
        private authenticationService: AuthenticationService
    ) {
        userService.getCurrentUserTables().subscribe(tables => {
            this.tables = tables;
        });
        userService.getCurrentUserTeamsTables().subscribe(teams => {
            this.teams = teams;
        });
    }

    titleUrlConverter(title: string): string{
        return title.replace(/\s/g, "-");
    }

    get currentUser() {
        return this.authenticationService.currentUser;
    }
}
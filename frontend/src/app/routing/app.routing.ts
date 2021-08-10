import { Routes, RouterModule } from '@angular/router';
import { HomeComponent } from '../components/home/home.component';
import { LoginComponent } from '../components/login/login.component';
import { TeamTablesComponent} from '../components/team-tables/team-tables.component'
import { SignupComponent } from '../components/signup/signup.component';
import { TableComponent } from '../components/table/table.component';
import { CreateTableComponent } from '../components/create-table/create-table.component';
import { TableCardsComponent } from '../components/table-cards/table-cards.component';
import { RestrictedComponent } from '../components/restricted/restricted.component';
import { UnknownComponent } from '../components/unknown/unknown.component';
import { MembersComponent } from '../components/members/members.component';
import { OrganizationMembersComponent } from '../components/organization-members/organization-members.component';
import { CreateOrganizationComponent} from '../components/create-organization/create-organization.component';

import { AuthGuard } from '../services/auth.guard';
import { Role } from '../models/user';
import { CreateTeamComponent } from '../components/create-team/create-team.component';
import { DeleteTeamComponent } from '../components/delete-team/delete-team.component';
import { DeleteUserOrgaComponent} from '../components/delete-userorga/delete-userorga.component';
import { DeleteOrganizationComponent} from '../components/delete-organization/delete-organization.component';
import { OrganizationDisplayTeamsComponent} from '../components/organization-displayteams/organization-displayteams.component';
import { OrganizationRemoveTeamComponent} from '../components/delete-teamorga/delete-teamorga.component';
import { SingleOrgaTableComponent} from '../components/single-orga-table/single-orga-table.component';
import { pathToFileURL } from 'url';

const appRoutes: Routes = [
  {
    path: 'table/:id/:title',
    component: TableCardsComponent,
    canActivate: [AuthGuard],
  },
  {
    path: 'table/create',
    component: CreateTableComponent,
    canActivate: [AuthGuard],
  },
  {
    path: ':id/tables',
    component: TeamTablesComponent,
    canActivate: [AuthGuard],
  },
  {
    path: 'login',
    component: LoginComponent,
  },
  {
    path: 'signup',
    component: SignupComponent
  },
  { path: '',
    component: HomeComponent,
    pathMatch: 'full',
  },
  {
    path: 'table',
    component: TableComponent,
    canActivate: [AuthGuard],
  },
  {
    path: 'team/:id/members',
    component: MembersComponent,
    canActivate: [AuthGuard],
  },
  {
    path: 'organization/:id/members',
    component: OrganizationMembersComponent,
    canActivate: [AuthGuard],
  },
  {
    path: 'organization/create',
    component: CreateOrganizationComponent,
    canActivate: [AuthGuard],
  },
  {
    path : 'organization/:id/tables',
    component: SingleOrgaTableComponent,
    canActivate: [AuthGuard],
  },
  { path: 'createTeam',
    component: CreateTeamComponent,
    canActivate: [AuthGuard],
  },
  { path: 'team/:id/delete',
    component: DeleteTeamComponent,
    canActivate: [AuthGuard],
  },
  {
    path : 'organization/confirm_delete/:id/:pseudo',
    component: DeleteUserOrgaComponent,
    canActivate: [AuthGuard],
  },
  {
    path: 'organization/confirm_delete/:id',
    component: DeleteOrganizationComponent,
    canActivate: [AuthGuard],
  },
  {
    path: 'organization/:id/teams',
    component: OrganizationDisplayTeamsComponent,
    canActivate: [AuthGuard],
  },
  {
    path: 'organization/confirm_delete/:orgId/team/:id',
    component: OrganizationRemoveTeamComponent,
    canActivate: [AuthGuard],
  },
  { path: 'restricted',
    component: RestrictedComponent,
    canActivate: [AuthGuard],
  },
  { path: '**',
    component: UnknownComponent,
  },
  {
    path: 'unknown',
    component: UnknownComponent,
  },
];

export const AppRoutes = RouterModule.forRoot(appRoutes);
import { ElementRef, EventEmitter, HostListener, Input, OnInit, Output, ViewChild } from '@angular/core';
import { Component } from '@angular/core';
import { User } from 'src/app/models/user';
import { CardService } from 'src/app/services/card.service';

@Component({
    selector: 'card-update-members',
    templateUrl: 'card-edit-member-add.component.html',
    styleUrls: ['card-edit-member-add.component.css']
})
export class CardEditMemberAddComponent implements OnInit{

    @Input() tableUsers: User[];
    @Input() cardUsers: User[];
    @Input() cardId: number;
    @Input() action: boolean;
    @Output() cardUsersChange = new EventEmitter<User>();
    @Output() closeInput = new EventEmitter<boolean>();
    @Output() userRemove = new EventEmitter<User>();
    joined: boolean[];
    @ViewChild("window") window: ElementRef;

    constructor(
        private cardService: CardService
    ) {
        this.joined = new Array();
    }

    ngOnInit(): void {
    }

    private actionIsLoaded(): boolean{
        console.log(typeof this.action !== 'undefined');
        console.log(this.action);
        return typeof this.action !== 'undefined';
    }

    private displayUsername(user: User): string {
        return user.pseudo;
    }

    private AlreadyJoined(userId: number): boolean{
        this.joined.push((this.cardUsers.includes(this.cardUsers.find(u => u.id == userId))));
        return this.joined[this.joined.length-1];
    }

    private closeRequest(): void{
        this.closeInput.emit(false);
    }

    private addMember(user: User){
        this.cardUsersChange.emit(user);
    }

    private deleteMember(user: User){
        this.userRemove.emit(user);
    }

    private stopPropagation(event){
        event.stopPropagation();
    }

    @HostListener('document:click', ['$event.target'])
    public onClick(targetElement) {
        if(!this.window.nativeElement.contains(targetElement)) {
            this.closeRequest();
        }   
    }
   
}
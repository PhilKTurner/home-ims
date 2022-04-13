import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { CatalogManagementRoutingModule } from './catalog-management-routing.module';
import { ArticleEditorComponent } from './article-editor/article-editor.component';
import { ArticleListComponent } from './article-list/article-list.component';


@NgModule({
  declarations: [
    ArticleEditorComponent,
    ArticleListComponent
  ],
  imports: [
    CommonModule,
    CatalogManagementRoutingModule
  ]
})
export class CatalogManagementModule { }

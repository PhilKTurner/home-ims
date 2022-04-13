import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { ArticleEditorComponent } from './article-editor/article-editor.component';
import { ArticleListComponent } from './article-list/article-list.component';

const routes: Routes = [
  { path: 'article-editor', component: ArticleEditorComponent },
  { path: 'article-list', component: ArticleListComponent }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class CatalogManagementRoutingModule { }

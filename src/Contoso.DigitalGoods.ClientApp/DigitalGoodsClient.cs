using AutoMapper;
using Contoso.DigitalGoods.Application;
using Contoso.DigitalGoods.TokenAPI.Proxy;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using QRCoder;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Contoso.DigitalGoods.ClientApp
{
    public partial class DigitalGoodsClient : Form
    {
        readonly Proxy _appproxy;
        readonly Client _tokenproxy;

        readonly IConfiguration _config;
        readonly IMapper _mapper;
        AppPropertyBag _propertyBag;


        public DigitalGoodsClient(HttpClient HttpClient, IConfiguration Config, AppPropertyBag AppPropertyBag, IMapper Mapper)
        {
            InitializeComponent();

            _appproxy = new Proxy(Config["App:ApplicationServiceEndPoint"], HttpClient);
            _tokenproxy = new Client(Config["App:TokenServiceEndPoint"], HttpClient);

            _config = Config;
            _propertyBag = AppPropertyBag;
            _mapper = Mapper;

            //Binding Configuration values with Control
            BindingControlWithConfigValuesPropertyBag();
            BindingControlWithUserProfilePropertyBag();

            this.cboType.SelectedIndex = 0;
            this.cboSubType.SelectedIndex = 0;
        }


        private void BindingControlWithConfigValuesPropertyBag()
        {
            this.txtContoso.DataBindings.Add(new Binding("Text", _propertyBag.Contoso.Profile, "NftServiceUserID"));
            this.txtContosoProductManager.DataBindings.Add(new Binding("Text", _propertyBag.ContosoProductManager.Profile, "NftServiceUserID"));
            this.txtContosoToken.DataBindings.Add(new Binding("Text", _propertyBag, "TokenID"));

            this.lblUserAContosoID.DataBindings.Add(new Binding("Text", _propertyBag.ContosoUserA.Profile, "ContosoID"));
            this.lblUserBContosoID.DataBindings.Add(new Binding("Text", _propertyBag.ContosoUserB.Profile, "ContosoID"));


        }

        private void BindingControlWithUserProfilePropertyBag()
        {
            this.lblUserAnftID.DataBindings.Clear();
            this.lblUserBnftID.DataBindings.Clear();
            this.lblUserAnftPublicKey.DataBindings.Clear();
            this.lblUserBnftPublicKey.DataBindings.Clear();

            this.lblUserAnftID.DataBindings.Add(new Binding("Text", _propertyBag.ContosoUserA.Profile, "NftServiceUserID"));
            this.lblUserBnftID.DataBindings.Add(new Binding("Text", _propertyBag.ContosoUserB.Profile, "NftServiceUserID"));
            this.lblUserAnftPublicKey.DataBindings.Add(new Binding("Text", _propertyBag.ContosoUserA.Profile, "PublicAddress"));
            this.lblUserBnftPublicKey.DataBindings.Add(new Binding("Text", _propertyBag.ContosoUserB.Profile, "PublicAddress"));
        }

        private async void BindingControlWithDigitalLockerPropertyBag()
        {
            await ProvisionAndRetriveDigitalLocker();
            this.grdUserADigitalLocker.DataSource = _propertyBag.ContosoUserA.DigitalLockerItems;
            this.grdUserBDigitalLocker.DataSource = _propertyBag.ContosoUserB.DigitalLockerItems;
        }

        private async void btnProvisionUsers_Click(object sender, EventArgs e)
        {
            await ProvisioningUsers();
            BindingControlWithUserProfilePropertyBag();
            BindingControlWithDigitalLockerPropertyBag();
            btnProvisionUsers.Enabled = false;
            btnCreateGift.Enabled = true;
        }

        private async Task ProvisioningUsers()
        {
            _propertyBag.ContosoUserA.Profile = (await _appproxy.UserPOSTAsync(_propertyBag.ContosoUserA.Profile.ContosoID)).Data;
            _propertyBag.ContosoUserB.Profile = (await _appproxy.UserPOSTAsync(_propertyBag.ContosoUserB.Profile.ContosoID)).Data;

            await ProvisionAndRetriveDigitalLocker();
        }

        private async Task ProvisionAndRetriveDigitalLocker()
        {
            _propertyBag.ContosoUserA.DigitalLockerItems = (await _appproxy.DigitalLockerAsync(_propertyBag.ContosoUserA.Profile.NftServiceUserID)).Data;
            _propertyBag.ContosoUserB.DigitalLockerItems = (await _appproxy.DigitalLockerAsync(_propertyBag.ContosoUserB.Profile.NftServiceUserID)).Data;
        }

        private async Task BindingControlWithProductCatalog()
        {
            _propertyBag.ProductCatalog = (await _appproxy.ProductsGET2Async()).Data;
            grdProductCatalog.DataSource = _propertyBag.ProductCatalog;
            grdFactoryProductCatalog.DataSource = _propertyBag.ProductCatalog;

            grdFactoryProductCatalog.Columns[0].Visible = false;
        }

        private async void btnRegisterProduct_Click(object sender, EventArgs e)
        {
            await RegisterProduct();
            await BindingControlWithProductCatalog();
            txtProductID.Text = Guid.NewGuid().ToString();
        }

        private async void btnRemoveProduct_Click(object sender, EventArgs e)
        {
            var selectedProduct = (ProductInfo)grdFactoryProductCatalog.CurrentRow.DataBoundItem;
            await _appproxy.ProductsDELETEAsync(selectedProduct.ProductID);

            await BindingControlWithProductCatalog();
        }

        private async Task RegisterProduct()
        {
            await _appproxy.ProductsPOSTAsync(new ProductInfo()
            {
                Id = Guid.NewGuid(),
                ProductID = txtProductID.Text,
                Title = txtProductTitle.Text,
                Subtitle = txtProductSubTitle.Text,
                Description = txtProductDescription.Text,
                Assets = new AssetInfo[] { new AssetInfo() {  Type =  (AssetType)cboType.SelectedIndex ,
                                                              SubType = (AssetSubType)cboSubType.SelectedIndex,
                                                              Url = txtProductImageURL.Text } }
            });
        }

        private async void tabDigitalGoods_SelectedIndexChanged(object sender, EventArgs e)
        {

            switch (tabDigitalGoods.SelectedIndex)
            {
                case 1:
                    BindingControlWithDigitalLockerPropertyBag();
                    break;

                case 2:
                    txtProductID.Text = Guid.NewGuid().ToString();
                    await BindingControlWithProductCatalog();
                    break;

                case 3:
                    await BindingControlWithProductInventory();
                    break;

                case 4:
                    await BindingControlWithGifts();
                    break;
                default:
                    break;
            }
        }

        private async void btnMintToken_Click(object sender, EventArgs e)
        {
            if (grdFactoryProductCatalog.CurrentRow == null) return;

            var mintedProduct = (ProductInfo)grdFactoryProductCatalog.CurrentRow.DataBoundItem;
            var result = await _tokenproxy.MakeDigitalGoodAsync(new DigitalKickToken()
            {
                ProductId = mintedProduct.ProductID,
                ProductName = mintedProduct.Title,
                ImageURL = mintedProduct.Assets.SingleOrDefault()?.Url,
                Model3DURL = "http://digitalmodelurl"
            });

            await BindingControlWithProductInventory();


        }

        private async Task BindingControlWithProductInventory()
        {
            _propertyBag.Contoso.DigitalLockerItems =
                        (await _appproxy.DigitalLockerAsync(_propertyBag.Contoso.Profile.NftServiceUserID)).Data;

            grdDigitalGoodInventory.DataSource = _propertyBag.Contoso.DigitalLockerItems;
            grdDigitalGoodGiftInventory.DataSource = _propertyBag.Contoso.DigitalLockerItems;
        }

        private async void btnCreateGift_Click(object sender, EventArgs e)
        {
            var mintedToken = (DigitalLockerItem)grdDigitalGoodGiftInventory.CurrentRow.DataBoundItem;
            var contosoId = rdoUserA.Checked ? _propertyBag.ContosoUserA.Profile.ContosoID : _propertyBag.ContosoUserB.Profile.ContosoID;

            if ((_propertyBag.Gifts != null) && (_propertyBag.Gifts.Where<DigitalGift>(x => x.TokenId == mintedToken.TokenNumber)).Count() > 0) return;

            await _appproxy.GiftsPOSTAsync(contosoId, mintedToken.TokenNumber);
            await BindingControlWithGifts();
        }

        private async Task BindingControlWithGifts()
        {
            var openGifts = (await _appproxy.GiftsGET2Async()).Data.Where<DigitalGift>(x => x.Status != "closed"
                                                                                            && (
                                                                                                (x.ReciverId.Contains(_propertyBag.ContosoUserA.Profile.ContosoID))
                                                                                                ||
                                                                                                (x.ReciverId.Contains(_propertyBag.ContosoUserB.Profile.ContosoID))
                                                                                                )
                                                                                       ).ToList();
            _propertyBag.Gifts = openGifts;

            grdGifts.DataSource = _propertyBag.Gifts;
            grdURLReadyGifts.DataSource = _propertyBag.Gifts;
        }

        private async void GenerateGiftURL(object sender, EventArgs e)
        {
            if ((_propertyBag.Gifts == null) || (_propertyBag.Gifts.Count == 0))
            {
                txtURL.Text = String.Empty;
                CreateQRCode(String.Empty);
                return;
            }

            var giftedToken = (DigitalGift)grdURLReadyGifts.CurrentRow.DataBoundItem;
            var tokenGiftURL = await _appproxy.GenerateURLAsync(giftedToken.GiftId);

            txtURL.Text = tokenGiftURL;
            CreateQRCode(tokenGiftURL);
        }

        /// <summary>
        /// Used QRCodeGenerator library from GitHub
        /// https://github.com/codebude/QRCoder/
        /// https://www.nuget.org/packages/QRCoder/1.4.3#show-readme-container
        /// </summary>
        /// <param name="tokenGiftURL"></param>
        private void CreateQRCode(string tokenGiftURL)
        {
            QRCodeGenerator qrCodeGen = new QRCodeGenerator();
            QRCodeData qrCodeData = qrCodeGen.CreateQrCode(tokenGiftURL, QRCodeGenerator.ECCLevel.Q);

            QRCode qrCode = new QRCode(qrCodeData);
            Bitmap qrCodeImage = qrCode.GetGraphic(20);

            QRTokenGiftURL.Image = qrCodeImage;
            QRTokenGiftURL.SizeMode = PictureBoxSizeMode.StretchImage;
        }

        private void NaviateToURL(object sender, EventArgs e)
        {
            try
            {
                System.Diagnostics.Process.Start(txtURL.Text);
            }
            catch (System.ComponentModel.Win32Exception noBrowser)
            {
                if (noBrowser.ErrorCode == -2147467259)
                    MessageBox.Show(noBrowser.Message);
            }
            catch (System.Exception other)
            {
                MessageBox.Show(other.Message);
            }
        }

        private async void RefreshGiftInfos(object sender, EventArgs e)
        {
            await BindingControlWithGifts();
        }

        private void ShowTooltip_grdUserADigitalLocker(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if ((e.ColumnIndex == 0) && (e.Value != null))
            {
                grdUserADigitalLocker.Rows[e.RowIndex].Cells[e.ColumnIndex].ToolTipText = 
                    JsonConvert.SerializeObject(((DigitalLockerItem)grdUserADigitalLocker.CurrentRow.DataBoundItem).ProductDetail, Formatting.Indented);
            }
        }

        private void ShowTooltip_grdUserBDigitalLocker(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if ((e.ColumnIndex == 0) && (e.Value != null))
            {
                grdUserBDigitalLocker.Rows[e.RowIndex].Cells[e.ColumnIndex].ToolTipText =
                    JsonConvert.SerializeObject(((DigitalLockerItem)grdUserBDigitalLocker.CurrentRow.DataBoundItem).ProductDetail, Formatting.Indented);
            }
        }
    }
}
